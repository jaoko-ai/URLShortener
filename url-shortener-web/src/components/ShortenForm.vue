<script setup lang="ts">
import { computed, ref } from "vue";
import { useLinksStore } from "../stores/links";

const links = useLinksStore();
const longUrl = ref("");
const customCode = ref("");
const showCustom = ref(false);
const copied = ref(false);

const urlPattern =
  /^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&/=]*)$/;

const localError = ref("");

const result = computed(() => links.lastCreated);

async function copyResult() {
  if (!result.value?.shortUrl) return;
  await navigator.clipboard.writeText(result.value.shortUrl);
  copied.value = true;
  window.setTimeout(() => {
    copied.value = false;
  }, 1800);
}

async function onSubmit() {
  localError.value = "";
  const value = longUrl.value.trim();

  if (!value) {
    localError.value = "Paste a URL first.";
    return;
  }
  if (!urlPattern.test(value)) {
    localError.value = "Use a full http or https URL.";
    return;
  }

  try {
    await links.shorten({
      longUrl: value,
      customCode: customCode.value.trim() || undefined,
    });
    longUrl.value = "";
    customCode.value = "";
  } catch {
    // Store already captured the API message.
  }
}
</script>

<template>
  <section class="rounded-3xl border border-line bg-card p-6 shadow-[0_24px_50px_-32px_rgba(28,25,20,0.45)] sm:p-8">
    <p class="text-xs uppercase tracking-[0.22em] text-ember">Create a short link</p>
    <h2 class="mt-2 font-display text-3xl leading-tight sm:text-4xl">
      Cut the long path down to size.
    </h2>
    <p class="mt-3 max-w-xl text-muted">
      Paste a destination, optionally pick a custom code, and Shortpath will ask the API
      for a compact redirect.
    </p>

    <form class="mt-6 space-y-4" @submit.prevent="onSubmit">
      <label class="block">
        <span class="mb-1.5 block text-sm text-muted">Destination URL</span>
        <input
          v-model="longUrl"
          type="url"
          autocomplete="off"
          placeholder="https://example.com/very/long/path"
          class="w-full rounded-2xl border border-line bg-paper px-4 py-3 outline-none ring-ember/30 transition focus:ring-2"
        />
      </label>

      <div class="flex flex-wrap items-center gap-3">
        <button
          type="button"
          class="text-sm text-pine underline decoration-pine/30 underline-offset-4"
          @click="showCustom = !showCustom"
        >
          {{ showCustom ? "Hide custom code" : "Add a custom code" }}
        </button>
        <button
          type="submit"
          class="ml-auto rounded-full bg-ember px-5 py-2.5 text-card transition hover:bg-ember-dark disabled:opacity-60"
          :disabled="links.loading"
        >
          {{ links.loading ? "Shortening…" : "Shorten" }}
        </button>
      </div>

      <label v-if="showCustom" class="block">
        <span class="mb-1.5 block text-sm text-muted">Custom code (optional)</span>
        <input
          v-model="customCode"
          type="text"
          maxlength="32"
          placeholder="my-launch"
          class="w-full rounded-2xl border border-line bg-paper px-4 py-3 outline-none ring-ember/30 transition focus:ring-2"
        />
      </label>
    </form>

    <p v-if="localError || links.error" class="mt-4 text-sm text-ember-dark">
      {{ localError || links.error }}
    </p>

    <div
      v-if="result"
      class="mt-6 flex flex-col gap-3 rounded-2xl border border-dashed border-moss/40 bg-paper p-4 sm:flex-row sm:items-center"
    >
      <div class="min-w-0 flex-1">
        <p class="text-xs uppercase tracking-widest text-muted">Short URL</p>
        <a
          :href="result.shortUrl"
          target="_blank"
          rel="noreferrer"
          class="mt-1 block truncate font-display text-xl text-pine"
        >
          {{ result.shortUrl }}
        </a>
        <p class="mt-1 truncate text-sm text-muted">{{ result.originalUrl }}</p>
      </div>
      <button
        type="button"
        class="rounded-full border border-pine px-4 py-2 text-sm text-pine hover:bg-pine hover:text-card"
        @click="copyResult"
      >
        {{ copied ? "Copied" : "Copy" }}
      </button>
    </div>
  </section>
</template>
