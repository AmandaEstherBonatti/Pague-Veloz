import api from '../config/api';
import type {
  Cliente,
  Conta,
  Transacao,
  Saldo,
  Extrato,
  LoginRequest,
  LoginResponse,
  CriarClienteRequest,
  CriarContaRequest,
  ProcessarTransacaoRequest,
} from '../types';

export const authService = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/clientes/login', data);
    return response.data;
  },

  criarCliente: async (data: CriarClienteRequest): Promise<Cliente> => {
    const response = await api.post<Cliente>('/clientes', data);
    return response.data;
  },
};

export const contaService = {
  criarConta: async (data: CriarContaRequest): Promise<Conta> => {
    const response = await api.post<Conta>('/contas', data);
    return response.data;
  },

  obterContas: async (clienteId: string): Promise<Conta[]> => {
    const response = await api.get<Conta[]>(`/contas/cliente/${clienteId}`);
    return response.data;
  },

  obterConta: async (id: string): Promise<Conta> => {
    const response = await api.get<Conta>(`/contas/${id}`);
    return response.data;
  },

  consultarSaldo: async (id: string): Promise<Saldo> => {
    const response = await api.get<Saldo>(`/contas/${id}/saldo`);
    return response.data;
  },
};

export const transacaoService = {
  processarTransacao: async (data: ProcessarTransacaoRequest): Promise<Transacao> => {
    const operationMap: Record<string, string> = {
      Credito: 'credit',
      Crédito: 'credit',
      Debito: 'debit',
      Débito: 'debit',
      Reserva: 'reserve',
      Captura: 'capture',
      Transferencia: 'transfer',
      Transferência: 'transfer',
      Estorno: 'reversal',
    };

    const operation = operationMap[data.tipo] ?? data.tipo.toLowerCase();
    const amountInCents = Math.round(data.valor * 100);

    const payload: Record<string, unknown> = {
      operation,
      accountId: data.contaId,
      amount: amountInCents,
      currency: 'BRL',
      referenceId: data.referenceId,
    };

    if (data.descricao) {
      payload.metadata = { description: data.descricao };
    }

    if (data.contaDestinoId) {
      payload.accountDestinationId = data.contaDestinoId;
    }

    const response = await api.post<Transacao>('/transacoes', payload);
    return response.data;
  },

  estornarTransacao: async (id: string, referenceId: string): Promise<Transacao> => {
    const response = await api.post<Transacao>(`/transacoes/${id}/estornar`, { referenceId });
    return response.data;
  },

  obterExtrato: async (
    contaId: string,
    dataInicio?: Date,
    dataFim?: Date
  ): Promise<Extrato> => {
    const params = new URLSearchParams();
    if (dataInicio) params.append('dataInicio', dataInicio.toISOString());
    if (dataFim) params.append('dataFim', dataFim.toISOString());

    const response = await api.get<Extrato>(
      `/transacoes/conta/${contaId}/extrato?${params.toString()}`
    );
    return response.data;
  },
};

