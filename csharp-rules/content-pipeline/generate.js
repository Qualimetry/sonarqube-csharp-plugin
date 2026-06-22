#!/usr/bin/env node
'use strict';

const fs = require('fs');
const path = require('path');

const repoRoot = path.resolve(__dirname, '..', '..');
const inventoryPath = path.join(repoRoot, 'docs', 'plans', 'rule-inventory.json');
const authoredDir = path.join(__dirname, 'authored');

const outRulesJson = path.join(repoRoot, 'csharp-rules', 'rules.json');
const outHtmlDir = path.join(repoRoot, 'csharp-rules', 'html');
const outRulesDocDir = path.join(repoRoot, 'docs', 'rules');
const outDescriptors = path.join(repoRoot, 'csharp-engine', 'Qualimetry.CSharp.Analyzer', 'Generated', 'Descriptors.cs');
const outGlobalConfig = path.join(repoRoot, 'csharp-engine', 'Qualimetry.CSharp.Analyzer', 'config', 'analyzers.globalconfig');

const HELP_BASE = 'https://github.com/Qualimetry/sonarqube-csharp-plugin/blob/main/docs/rules/';

const PROFILE_CATEGORY = {
  'Default Code Quality profile': 'CodeQuality',
  'Optional Code Quality profile': 'CodeQuality',
  'Optional Style profile': 'Style',
  'Optional Naming profile': 'Naming',
  'Optional Metrics profile': 'Metrics',
  'Default Security/Reliability profile': 'Reliability',
  'Optional Unity profile': 'Unity',
  'Optional Contract profile': 'Contract',
  'Optional Interop profile': 'Interop',
};

function fail(msg) {
  console.error('content-pipeline: ' + msg);
  process.exit(1);
}

function readJson(p) {
  let text = fs.readFileSync(p, 'utf8');
  if (text.charCodeAt(0) === 0xfeff) text = text.slice(1);
  return JSON.parse(text);
}

function htmlEscape(s) {
  return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function severityToken(standardSeverity, defaultActive) {
  if (!defaultActive) return 'none';
  switch (standardSeverity) {
    case 'BLOCKER':
    case 'CRITICAL':
    case 'MAJOR':
      return 'warning';
    case 'MINOR':
      return 'suggestion';
    case 'INFO':
      return 'silent';
    default:
      return 'warning';
  }
}

function defaultSeverityEnum(standardSeverity) {
  switch (standardSeverity) {
    case 'BLOCKER':
    case 'CRITICAL':
    case 'MAJOR':
      return 'Warning';
    case 'MINOR':
      return 'Info';
    case 'INFO':
      return 'Hidden';
    default:
      return 'Warning';
  }
}

function csharpString(s) {
  return '"' + String(s).replace(/\\/g, '\\\\').replace(/"/g, '\\"').replace(/\r?\n/g, '\\n') + '"';
}

function paramTypeToken(type) {
  switch (String(type).trim().toLowerCase()) {
    case 'int':
    case 'integer':
      return 'INTEGER';
    case 'bool':
    case 'boolean':
      return 'BOOLEAN';
    case 'text':
      return 'TEXT';
    default:
      return 'STRING';
  }
}

function paramDescription(c) {
  if (c.description) return c.description;
  if (paramTypeToken(c.type) === 'INTEGER') {
    return 'Threshold the rule compares against; measurements above it are reported. Default: ' + c.default + '.';
  }
  return 'Value the rule applies when evaluating code. Default: ' + c.default + '.';
}

function buildParams(authored) {
  if (!authored.configuration || !authored.configuration.length) return undefined;
  return authored.configuration.map((c) => ({
    key: c.parameter,
    name: c.name || c.parameter,
    type: paramTypeToken(c.type),
    defaultValue: String(c.default),
    description: paramDescription(c),
  }));
}

function plainDescription(authored) {
  const first = (authored.summary && authored.summary[0]) || '';
  return first.replace(/<[^>]+>/g, '').replace(/\s+/g, ' ').trim();
}

function buildHtml(authored) {
  const parts = [];
  for (const p of authored.summary || []) parts.push(p);
  parts.push('<h2>Noncompliant Code Example</h2>');
  parts.push('<pre>\n' + htmlEscape(authored.noncompliant) + '\n</pre>');
  parts.push('<h2>Compliant Solution</h2>');
  parts.push('<pre>\n' + htmlEscape(authored.compliant) + '\n</pre>');
  if (authored.configuration && authored.configuration.length) {
    parts.push('<h2>Parameters</h2>');
    const names = authored.configuration.map((c) => '<code>' + htmlEscape(c.parameter) + '</code> (default <code>' + htmlEscape(c.default) + '</code>)');
    parts.push('<p>This rule is configurable. Edit ' + names.join(', ') +
      ' on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.</p>');
  }
  if (authored.seeAlso && authored.seeAlso.length) {
    parts.push('<h2>See Also</h2>');
    parts.push('<ul>');
    for (const s of authored.seeAlso) {
      if (s.href) parts.push('<li><a href="' + htmlEscape(s.href) + '">' + htmlEscape(s.text) + '</a></li>');
      else parts.push('<li>' + htmlEscape(s.text) + '</li>');
    }
    parts.push('</ul>');
  }
  return parts.join('\n') + '\n';
}

function unescapeHtml(s) {
  return String(s)
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'")
    .replace(/&amp;/g, '&');
}

function summaryToMarkdown(summary) {
  const blocks = [];
  for (const entry of summary || []) {
    let s = String(entry)
      .replace(/<a[^>]*href="([^"]*)"[^>]*>([\s\S]*?)<\/a>/gi, (_m, href, t) => '[' + t + '](' + href + ')')
      .replace(/<code>([\s\S]*?)<\/code>/gi, (_m, t) => '`' + t + '`')
      .replace(/<\/?(strong|b)>/gi, '**')
      .replace(/<\/?(em|i)>/gi, '*');
    if (/<li>/i.test(s)) {
      s = s.replace(/<\/?ul>/gi, '').replace(/<li>([\s\S]*?)<\/li>/gi, (_m, t) => '- ' + t.trim() + '\n');
    }
    s = s.replace(/<\/?p>/gi, '').replace(/<[^>]+>/g, '');
    s = unescapeHtml(s).replace(/[ \t]+/g, ' ').trim();
    if (s) blocks.push(s);
  }
  return blocks.join('\n\n');
}

function buildMarkdown(rule) {
  const a = rule.authored;
  const out = [];
  out.push('# ' + rule.title);
  out.push('');
  out.push('`' + rule.id + '` &middot; ' + rule.category + ' &middot; ' + rule.type + ' &middot; severity ' + rule.severity +
    (rule.defaultActive ? ' &middot; enabled in the recommended profile' : ' &middot; optional'));
  out.push('');
  const body = summaryToMarkdown(a.summary);
  if (body) { out.push(body); out.push(''); }
  out.push('## Noncompliant code example');
  out.push('');
  out.push('```csharp');
  out.push(a.noncompliant);
  out.push('```');
  out.push('');
  out.push('## Compliant solution');
  out.push('');
  out.push('```csharp');
  out.push(a.compliant);
  out.push('```');
  if (a.configuration && a.configuration.length) {
    out.push('');
    out.push('## Parameters');
    out.push('');
    const names = a.configuration.map((c) => '`' + c.parameter + '` (default `' + c.default + '`)');
    out.push('This rule is configurable. Edit ' + names.join(', ') +
      ' on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.');
  }
  if (a.seeAlso && a.seeAlso.length) {
    out.push('');
    out.push('## See also');
    out.push('');
    for (const s of a.seeAlso) {
      out.push(s.href ? '- [' + s.text + '](' + s.href + ')' : '- ' + s.text);
    }
  }
  return out.join('\n').replace(/\n{3,}/g, '\n\n').trimEnd() + '\n';
}

function main() {
  if (!fs.existsSync(inventoryPath)) fail('missing inventory at ' + inventoryPath);
  const inventory = readJson(inventoryPath);
  const byId = new Map(inventory.map((r) => [r.id, r]));

  const authoredFiles = fs.existsSync(authoredDir)
    ? fs.readdirSync(authoredDir).filter((f) => f.endsWith('.json'))
    : [];

  const rules = [];
  const seenIds = new Set();
  for (const f of authoredFiles.sort()) {
    const authored = readJson(path.join(authoredDir, f));
    const inv = byId.get(authored.id);
    if (!inv) fail('authored ' + f + ' has id ' + authored.id + ' not present in inventory');
    if (seenIds.has(authored.id)) fail('duplicate id ' + authored.id);
    seenIds.add(authored.id);
    if (!authored.ruleKey) fail('authored ' + f + ' is missing ruleKey');
    const category = PROFILE_CATEGORY[inv.profile] || 'CodeQuality';
    rules.push({
      id: authored.ruleKey,
      key: authored.ruleKey,
      symbol: inv.id,
      title: authored.title,
      messageFormat: authored.messageFormat || authored.title,
      severity: inv.severity,
      type: inv.type,
      category,
      tags: authored.tags || [],
      profile: inv.profile,
      defaultActive: inv.defaultActive,
      helpUrl: HELP_BASE + authored.ruleKey + '.md',
      authored,
    });
  }
  rules.sort((a, b) => a.id.localeCompare(b.id));

  const outputs = [];

  // rules.json (catalogue - no vendor text, no authored bodies)
  const catalogue = rules.map((r) => {
    const entry = {
      id: r.id,
      key: r.key,
      title: r.title,
      severity: r.severity,
      type: r.type,
      category: r.category,
      tags: r.tags,
      defaultActive: r.defaultActive,
      helpUrl: r.helpUrl,
    };
    const parameters = buildParams(r.authored);
    if (parameters) entry.parameters = parameters;
    return entry;
  });
  outputs.push({ path: outRulesJson, content: JSON.stringify({ rules: catalogue }, null, 2) + '\n' });

  // html
  for (const r of rules) {
    outputs.push({ path: path.join(outHtmlDir, r.id + '.html'), content: buildHtml(r.authored) });
  }

  // per-rule markdown (the helpLinkUri / helpUrl targets)
  for (const r of rules) {
    outputs.push({ path: path.join(outRulesDocDir, r.id + '.md'), content: buildMarkdown(r) });
  }

  // Descriptors.cs
  const lines = [];
  lines.push('// <auto-generated> Generated by content-pipeline. Do not edit by hand. </auto-generated>');
  lines.push('using Microsoft.CodeAnalysis;');
  lines.push('');
  lines.push('namespace Qualimetry.CSharp.Analyzer;');
  lines.push('');
  lines.push('internal static class Descriptors');
  lines.push('{');
  for (const r of rules) {
    lines.push('    public static readonly DiagnosticDescriptor ' + r.symbol + ' = new(');
    lines.push('        id: ' + csharpString(r.id) + ',');
    lines.push('        title: ' + csharpString(r.title) + ',');
    lines.push('        messageFormat: ' + csharpString(r.messageFormat) + ',');
    lines.push('        category: ' + csharpString(r.category) + ',');
    lines.push('        defaultSeverity: DiagnosticSeverity.' + defaultSeverityEnum(r.severity) + ',');
    lines.push('        isEnabledByDefault: true,');
    lines.push('        description: ' + csharpString(plainDescription(r.authored)) + ',');
    lines.push('        helpLinkUri: ' + csharpString(r.helpUrl) + ');');
    lines.push('');
  }
  lines.push('}');
  outputs.push({ path: outDescriptors, content: lines.join('\n') + '\n' });

  // .globalconfig
  const gc = [];
  gc.push('is_global = true');
  gc.push('');
  for (const r of rules) {
    gc.push('dotnet_diagnostic.' + r.id + '.severity = ' + severityToken(r.severity, r.defaultActive));
  }
  outputs.push({ path: outGlobalConfig, content: gc.join('\n') + '\n' });

  const normaliseEol = (s) => s.replace(/\r\n/g, '\n');

  const checkOnly = process.argv.includes('--check');
  if (checkOnly) {
    const stale = [];
    for (const o of outputs) {
      const current = fs.existsSync(o.path) ? fs.readFileSync(o.path, 'utf8') : null;
      if (current === null || normaliseEol(current) !== normaliseEol(o.content)) {
        stale.push(path.relative(repoRoot, o.path));
      }
    }
    if (stale.length) {
      fail('generated artifacts are out of date; run the content-pipeline and commit:\n  ' + stale.join('\n  '));
    }
    console.log('content-pipeline: ' + outputs.length + ' generated artifacts match the catalogue (' +
      rules.length + ' rules)');
    return;
  }

  for (const dir of [outHtmlDir, outRulesDocDir, path.dirname(outRulesJson), path.dirname(outDescriptors), path.dirname(outGlobalConfig)]) {
    fs.mkdirSync(dir, { recursive: true });
  }
  let written = 0;
  for (const o of outputs) {
    const current = fs.existsSync(o.path) ? fs.readFileSync(o.path, 'utf8') : null;
    if (current !== null && normaliseEol(current) === normaliseEol(o.content)) continue;
    fs.writeFileSync(o.path, o.content);
    written++;
  }

  console.log('content-pipeline: generated ' + rules.length + ' rules (' +
    catalogue.filter((r) => r.defaultActive).length + ' default-active), ' +
    rules.length + ' html + ' + rules.length + ' markdown docs; ' + written + ' file(s) updated');
}

main();
