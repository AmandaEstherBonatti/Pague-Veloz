# PagueVeloz Frontend

Aplicação frontend React para o sistema PagueVeloz de processamento de transações financeiras.

## 🎨 Dashboard

O Dashboard exibe as informações mais importantes e relevantes para o usuário:

### 📊 Métricas Principais
- **Saldo Total**: Visão consolidada de todas as contas
- **Saldo Disponível**: Dinheiro disponível para uso imediato
- **Saldo Reservado**: Valores em reserva aguardando captura
- **Limite Disponível**: Limite de crédito + saldo disponível

### 📈 Resumo do Mês
- **Receitas do Mês**: Total de créditos recebidos
- **Despesas do Mês**: Total de débitos e transferências
- **Saldo do Mês**: Diferença entre receitas e despesas

### 💼 Minhas Contas
- Lista de todas as contas do usuário
- Saldo disponível, reservado e total por conta
- Status da conta (Ativa, Bloqueada, etc.)
- Link direto para detalhes de cada conta

### 📋 Últimas Transações
- As 10 transações mais recentes
- Tipo, valor, status e data
- Indicadores visuais por tipo de transação
- Alertas de erros quando aplicável

### ⚡ Ações Rápidas
- Nova Transação
- Nova Conta
- Ver Histórico
- Configurações

## 🚀 Tecnologias

- **React 18** com TypeScript
- **Vite** - Build tool rápida
- **React Router** - Roteamento
- **Axios** - Cliente HTTP
- **Tailwind CSS** - Estilização moderna
- **Lucide React** - Ícones
- **date-fns** - Formatação de datas

## 📦 Instalação

```bash
npm install
```

## 🏃 Executar

```bash
npm run dev
```

A aplicação estará disponível em `http://localhost:3000`

## 🔧 Configuração

Crie um arquivo `.env` na raiz do projeto:

```
VITE_API_BASE_URL=http://localhost:5232/api
```

## 🔐 Autenticação

A aplicação usa JWT tokens armazenados no localStorage. O token é automaticamente incluído em todas as requisições através dos interceptors do Axios.

## 📱 Funcionalidades do Dashboard

✅ Visão consolidada de saldos
✅ Resumo financeiro do mês
✅ Lista de contas com saldos
✅ Últimas transações em tempo real
✅ Ações rápidas para operações comuns
✅ Design responsivo e moderno
✅ Indicadores visuais por tipo de transação

