import { createContext, PropsWithChildren, useContext, useEffect, useMemo, useState } from 'react';

import { ApiClient } from '../services/ApiClient';
import { clearSessionValue, readSessionValue, writeSessionValue } from '../services/storage';
import { Session } from '../types/models';

interface AuthValue {
  session: Session | null;
  restoring: boolean;
  api: ApiClient;
  signIn(login: string, password: string): Promise<void>;
  signOut(): Promise<void>;
}

const AuthContext = createContext<AuthValue | null>(null);

export function AuthProvider({ children }: PropsWithChildren) {
  const [session, setSession] = useState<Session | null>(null);
  const [restoring, setRestoring] = useState(true);
  const api = useMemo(() => new ApiClient(() => session?.token ?? null), [session?.token]);

  useEffect(() => {
    readSessionValue()
      .then((raw) => {
        if (!raw) return;
        const restored = JSON.parse(raw) as Session;
        if (!restored.token || (restored.expiraEnUtc && new Date(restored.expiraEnUtc) <= new Date())) return;
        setSession(restored);
      })
      .catch(() => undefined)
      .finally(() => setRestoring(false));
  }, []);

  async function signIn(login: string, password: string) {
    const next = await api.login(login.trim(), password);
    if (!next.token) throw new Error('El servidor no devolvió un token válido.');
    if (!next.roles.some((role) => role.toUpperCase() === 'CLIENTE')) {
      throw new Error('Esta aplicación está disponible para clientes registrados.');
    }
    await writeSessionValue(JSON.stringify(next));
    setSession(next);
  }

  async function signOut() {
    await clearSessionValue();
    setSession(null);
  }

  return (
    <AuthContext.Provider value={{ session, restoring, api, signIn, signOut }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth debe utilizarse dentro de AuthProvider.');
  return value;
}
