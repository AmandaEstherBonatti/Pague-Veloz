import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { authService } from '../services/api';
import type { LoginResponse, Cliente } from '../types';

interface AuthContextType {
  cliente: Cliente | null;
  token: string | null;
  login: (usuario: string, senha: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [cliente, setCliente] = useState<Cliente | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedCliente = localStorage.getItem('cliente');

    if (storedToken && storedCliente) {
      setToken(storedToken);
      setCliente(JSON.parse(storedCliente));
    }
  }, []);

  const login = async (usuario: string, senha: string) => {
    const response: LoginResponse = await authService.login({ usuario, senha });
    
    setToken(response.token);
    setCliente({
      id: response.clienteId,
      nome: response.nome,
      email: response.email,
      usuario: usuario,
      autenticacaoMultiFatorAtiva: response.requerAutenticacaoMultiFator,
      dataCriacao: new Date().toISOString(),
      ativo: true,
    });

    localStorage.setItem('token', response.token);
    localStorage.setItem('cliente', JSON.stringify({
      id: response.clienteId,
      nome: response.nome,
      email: response.email,
      usuario: usuario,
      autenticacaoMultiFatorAtiva: response.requerAutenticacaoMultiFator,
      dataCriacao: new Date().toISOString(),
      ativo: true,
    }));
  };

  const logout = () => {
    setToken(null);
    setCliente(null);
    localStorage.removeItem('token');
    localStorage.removeItem('cliente');
  };

  return (
    <AuthContext.Provider
      value={{
        cliente,
        token,
        login,
        logout,
        isAuthenticated: !!token,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};

