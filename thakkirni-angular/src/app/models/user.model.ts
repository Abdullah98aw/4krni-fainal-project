export interface User {
  id: number;
  name: string;
  email: string;
  nationalId?: string;
<<<<<<< HEAD
  role: 'ADMIN' | 'MANAGER' | 'USER';
  avatar?: string;
  jobTitle?: string;
  password?: string;
=======
  role: 'ADMIN' | 'USER';
  avatar?: string;
  jobTitle?: string;
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
