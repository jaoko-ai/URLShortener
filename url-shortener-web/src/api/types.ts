export interface User {
  id: string;
  username: string;
  email: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ShortenRequest {
  longUrl: string;
  customCode?: string;
}

export interface ShortLink {
  id: number;
  originalUrl: string;
  shortCode: string;
  shortUrl: string;
  createdAt: string;
  clickCount: number;
}

export interface ClickEvent {
  id: number;
  timestamp: string;
  referrer: string;
  userAgent?: string | null;
  ipAddress?: string | null;
  language?: string | null;
}

export interface CountBucket {
  label: string;
  count: number;
}

export interface LinkStats {
  shortCode: string;
  originalUrl: string;
  clickCount: number;
  clicks: ClickEvent[];
  byDay: CountBucket[];
  byReferrer: CountBucket[];
}

export interface ApiErrorBody {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
}
