import axios, { AxiosError, type AxiosInstance } from "axios";
import type { ApiErrorBody } from "./types";

export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5062";

export const PUBLIC_SHORT_BASE_URL =
  import.meta.env.VITE_PUBLIC_SHORT_BASE_URL ?? API_BASE_URL;

const TOKEN_KEY = "shortpath.token";

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setStoredToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
}

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 15_000,
});

apiClient.interceptors.request.use((config) => {
  const token = getStoredToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<ApiErrorBody | string>;
    const data = axiosError.response?.data;

    if (typeof data === "string" && data.trim()) {
      return data;
    }

    if (data && typeof data === "object") {
      if (data.message) return data.message;
      if (data.title) return data.title;
      if (data.errors) {
        const first = Object.values(data.errors).flat()[0];
        if (first) return first;
      }
    }

    if (axiosError.code === "ERR_NETWORK") {
      return `Cannot reach the API at ${API_BASE_URL}. Is the backend running?`;
    }

    if (axiosError.response?.status) {
      return `Request failed (${axiosError.response.status}).`;
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Something went wrong.";
}
