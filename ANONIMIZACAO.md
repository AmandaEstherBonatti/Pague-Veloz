# 🔒 Guia de Anonimização da Solução

Este documento fornece instruções para anonimizar a solução antes da submissão, removendo todas as informações pessoais.

## ⚠️ Importante

Antes de submeter a solução, você **DEVE** remover todas as informações pessoais identificáveis, incluindo:

- ✅ Nomes de pessoas
- ✅ Endereços de email
- ✅ Informações de autoria
- ✅ Caminhos de usuário do sistema
- ✅ Comentários pessoais no código
- ✅ Metadados de editores
- ✅ Configurações de controle de versão com informações pessoais

## 📋 Checklist de Anonimização

### 1. Arquivos de Código
- [ ] Remover comentários com nomes ou emails
- [ ] Remover comentários automáticos de editores (ex: "Created by Visual Studio")
- [ ] Verificar se não há caminhos de usuário hardcoded (ex: `C:\Users\SeuNome\`)
- [ ] Verificar namespaces - devem ser genéricos (ex: `PagueVeloz.*`)

### 2. Documentação
- [ ] Verificar `README.md` - remover seção de "Autores" ou informações pessoais
- [ ] Verificar outros arquivos `.md` na raiz e em `docs/`
- [ ] Remover referências a nomes pessoais

### 3. Configuração Git
- [ ] Verificar `.git/config` - não deve conter informações pessoais
- [ ] O histórico Git não será incluído no ZIP (usando `git archive`)

### 4. Arquivos de Configuração
- [ ] Verificar `.csproj.user` - pode conter configurações pessoais
- [ ] Verificar `launchSettings.json` - deve conter apenas configurações genéricas
- [ ] Verificar se há arquivos `.env` ou similares (devem estar no `.gitignore`)

### 5. Logs e Arquivos Temporários
- [ ] Verificar pasta `logs/` - não deve conter informações pessoais
- [ ] Verificar se arquivos temporários estão no `.gitignore`

## 🚀 Gerar Arquivo ZIP Anonimizado

### Opção 1: Usando Script (Recomendado)

#### Windows (PowerShell)
```powershell
.\scripts\export-anonymized.ps1
```

#### Linux/Mac (Bash)
```bash
chmod +x scripts/export-anonymized.sh
./scripts/export-anonymized.sh
```

### Opção 2: Comando Git Manual

```bash
# Na raiz do repositório
git archive --format=zip --output=./pagueveloz-challenge.zip HEAD
```

## ✅ Verificações Finais

Antes de submeter o arquivo ZIP:

1. **Extraia o ZIP** e verifique se não há informações pessoais
2. **Pesquise por seu nome** nos arquivos extraídos
3. **Pesquise por seu email** nos arquivos extraídos
4. **Verifique caminhos** - não deve haver caminhos de usuário
5. **Confirme que `.env` não está incluído** no ZIP
6. **Verifique logs** - não devem conter informações pessoais

## 🔍 Comandos Úteis para Verificação

### Pesquisar por nome ou email no código
```bash
# Linux/Mac/Git Bash
grep -r "seu-nome" . --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj
grep -r "seu-email@exemplo.com" . --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj

# PowerShell
Select-String -Path .\* -Pattern "seu-nome" -Recurse -Exclude node_modules,bin,obj
Select-String -Path .\* -Pattern "seu-email@exemplo.com" -Recurse -Exclude node_modules,bin,obj
```

### Verificar arquivos que serão incluídos no ZIP
```bash
# Listar arquivos que serão incluídos
git ls-tree -r HEAD --name-only
```

### Verificar tamanho do ZIP
```bash
# Linux/Mac
ls -lh pagueveloz-challenge.zip

# Windows PowerShell
(Get-Item pagueveloz-challenge.zip).Length / 1MB
```

## 📝 Notas Importantes

1. **O comando `git archive`** cria um ZIP apenas com os arquivos versionados, **sem** o histórico Git
2. **Arquivos no `.gitignore`** não serão incluídos no ZIP
3. **Certifique-se** de que todos os arquivos sensíveis estão no `.gitignore`
4. **O arquivo ZIP gerado** também está no `.gitignore` para não ser commitado acidentalmente

## 🎯 Estrutura Esperada no ZIP

O arquivo ZIP deve conter:

```
pagueveloz-challenge.zip
├── PagueVeloz/              # Backend
├── PagueVeloz.Frontend/     # Frontend
├── nginx/                   # Configuração Nginx
├── scripts/                 # Scripts auxiliares
├── docker-compose*.yml      # Arquivos Docker
├── README.md                # Documentação
├── .gitignore              # Git ignore
└── ...                     # Outros arquivos necessários
```

**NÃO deve conter:**
- ❌ `.git/` (histórico Git)
- ❌ `node_modules/`
- ❌ `bin/`, `obj/`
- ❌ `.env` ou arquivos sensíveis
- ❌ `logs/` com dados pessoais
- ❌ Arquivos temporários

---

**✅ Após seguir este guia, sua solução estará anonimizada e pronta para submissão!**

