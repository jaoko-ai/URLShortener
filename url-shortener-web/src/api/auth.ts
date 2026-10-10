import { apiClient } from "./client";
import type { AuthResponse, LoginRequest, RegisterRequest, User } from "./types";

export async function register(payload: RegisterRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/api/auth/register", payload);
  return data;
}

export async function login(payload: LoginRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/api/auth/login", payload);
  return data;
}

export async function logout(): Promise<void> {
  await apiClient.post("/api/auth/logout");
}

export async function getMe(): Promise<User> {
  const { data } = await apiClient.get<User>("/api/auth/me");
  return data;
}
