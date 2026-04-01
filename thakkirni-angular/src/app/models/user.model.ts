export interface User {
  id: number;
  name: string;
  email: string;
  nationalId?: string;
  role: 'ADMIN' | 'USER';
  avatar?: string;
  jobTitle?: string;
  agencyId?: number;
  agencyName?: string;
  departmentId?: number;
  departmentName?: string;
  sectionId?: number;
  sectionName?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}
