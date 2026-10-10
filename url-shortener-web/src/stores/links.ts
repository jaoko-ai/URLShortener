import { defineStore } from "pinia";
import { computed, ref } from "vue";
import { getErrorMessage, PUBLIC_SHORT_BASE_URL } from "../api/client";
import * as urlsApi from "../api/urls";
import type { LinkStats, ShortenRequest, ShortLink } from "../api/types";

const RECENT_KEY = "shortpath.recent";

function readRecent(): ShortLink[] {
  try {
    const raw = localStorage.getItem(RECENT_KEY);
    return raw ? (JSON.parse(raw) as ShortLink[]) : [];
  } catch {
    return [];
  }
}

function writeRecent(links: ShortLink[]) {
  localStorage.setItem(RECENT_KEY, JSON.stringify(links.slice(0, 20)));
}

export const useLinksStore = defineStore("links", () => {
  const links = ref<ShortLink[]>(readRecent());
  const lastCreated = ref<ShortLink | null>(null);
  const selectedStats = ref<LinkStats | null>(null);
  const selectedCode = ref("");
  const loading = ref(false);
  const statsLoading = ref(false);
  const error = ref("");

  const totalClicks = computed(() =>
    links.value.reduce((sum, link) => sum + (link.clickCount ?? 0), 0),
  );

  function upsert(link: ShortLink) {
    const next = [link, ...links.value.filter((item) => item.shortCode !== link.shortCode)];
    links.value = next;
    writeRecent(next);
  }

  async function shorten(payload: ShortenRequest) {
    loading.value = true;
    error.value = "";
    try {
      const created = await urlsApi.shortenUrl(payload);
      if (!created.shortUrl) {
        created.shortUrl = `${PUBLIC_SHORT_BASE_URL.replace(/\/$/, "")}/${created.shortCode}`;
      }
      lastCreated.value = created;
      upsert(created);
      return created;
    } catch (err) {
      error.value = getErrorMessage(err);
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function loadMine() {
    loading.value = true;
    error.value = "";
    try {
      const mine = await urlsApi.listMyUrls();
      links.value = mine;
      writeRecent(mine);
    } catch (err) {
      error.value = getErrorMessage(err);
    } finally {
      loading.value = false;
    }
  }

  async function loadStats(shortCode: string) {
    statsLoading.value = true;
    error.value = "";
    selectedCode.value = shortCode;
    try {
      selectedStats.value = await urlsApi.getUrlStats(shortCode);
    } catch (err) {
      selectedStats.value = null;
      error.value = getErrorMessage(err);
    } finally {
      statsLoading.value = false;
    }
  }

  async function remove(shortCode: string) {
    error.value = "";
    try {
      await urlsApi.deleteUrl(shortCode);
      links.value = links.value.filter((link) => link.shortCode !== shortCode);
      writeRecent(links.value);
      if (selectedCode.value === shortCode) {
        selectedStats.value = null;
        selectedCode.value = "";
      }
    } catch (err) {
      error.value = getErrorMessage(err);
      throw err;
    }
  }

  return {
    links,
    lastCreated,
    selectedStats,
    selectedCode,
    loading,
    statsLoading,
    error,
    totalClicks,
    shorten,
    loadMine,
    loadStats,
    remove,
  };
});
