export interface User {
  id: number;
  name: string;
  email: string;
  nationalId?: string;
  role: 'ADMIN' | 'USER';
  avatar?: string;
  departmentId?: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}
