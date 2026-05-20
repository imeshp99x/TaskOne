export interface User {
  id: number;
  username: string;
  fullName: string;
  email?: string;
}

export interface CreateUserDto {
  username: string;
  password: string;
  fullName: string;
  email?: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  fullName: string;
}
