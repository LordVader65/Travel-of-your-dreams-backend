export interface LoginRequest {
  login: string;
  password: string;
}

export interface LoginResponse {
  usuario_guid: string;
  cliente_guid?: string | null;
  login: string;
  roles: string[];
  token: string;
  expira_en_utc: string;
}
