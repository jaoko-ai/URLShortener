import { apiClient } from "./client";
import type { LinkStats, ShortenRequest, ShortLink } from "./types";

export async function shortenUrl(payload: ShortenRequest): Promise<ShortLink> {
  const { data } = await apiClient.post<ShortLink>("/api/url/shorten", payload);
  return data;
}

export async function listMyUrls(): Promise<ShortLink[]> {
  const { data } = await apiClient.get<ShortLink[]>("/api/url/mine");
  return data;
}

export async function getUrl(shortCode: string): Promise<ShortLink> {
  const { data } = await apiClient.get<ShortLink>(`/api/url/${encodeURIComponent(shortCode)}`);
  return data;
}

export async function getUrlStats(shortCode: string): Promise<LinkStats> {
  const { data } = await apiClient.get<LinkStats>(
    `/api/url/stats/${encodeURIComponent(shortCode)}`,
  );
  return data;
}

export async function deleteUrl(shortCode: string): Promise<void> {
  await apiClient.delete(`/api/url/${encodeURIComponent(shortCode)}`);
}
