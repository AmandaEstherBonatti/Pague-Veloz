export interface Cliente {
  id: string;
  nome: string;
  email: string;
  usuario: string;
  autenticacaoMultiFatorAtiva: boolean;
  dataCriacao: string;
  ativo: boolean;
}

export interface Conta {
  id: string;
  clienteId: string;
  numero: string;
  saldoDisponivel: number;
  saldoReservado: number;
  saldoTotal: number;
  limiteCredito: number;
  status: string;
  dataCriacao: string;
}

export interface Transacao {
  id: string;
  contaId: string;
  contaDestinoId?: string;
  referenceId: string;
  tipo: string;
  valor: number;
  status: string;
  descricao?: string;
  dataCriacao: string;
  dataProcessamento?: string;
  erro?: string;
}

export interface Saldo {
  contaId: string;
  numeroConta: string;
  saldoDisponivel: number;
  saldoReservado: number;
  saldoTotal: number;
  limiteCredito: number;
  limiteDisponivel: number;
}

export interface Extrato {
  contaId: string;
  numeroConta: string;
  dataInicio: string;
  dataFim: string;
  transacoes: Transacao[];
  totalTransacoes: number;
}

export interface LoginRequest {
  usuario: string;
  senha: string;
}

export interface LoginResponse {
  clienteId: string;
  nome: string;
  email: string;
  token: string;
  requerAutenticacaoMultiFator: boolean;
}

export interface CriarClienteRequest {
  nome: string;
  email: string;
  usuario: string;
  senha: string;
}

export interface CriarContaRequest {
  clienteId: string;
  limiteCredito: number;
}

export interface ProcessarTransacaoRequest {
  contaId: string;
  referenceId: string;
  tipo: string;
  valor: number;
  descricao?: string;
  contaDestinoId?: string;
}

