<script setup lang="ts">
import { computed } from "vue";
import { useLinksStore } from "../stores/links";

const links = useLinksStore();

const slices = computed(() => {
  const stats = links.selectedStats;
  if (!stats || stats.byReferrer.length === 0) {
    return [
      { label: "No data", count: 1, color: "#d9cbb0" },
    ];
  }
  const palette = ["#c45c26", "#244338", "#3f6b56", "#9c4016", "#6d6456"];
  return stats.byReferrer.slice(0, 5).map((bucket, index) => ({
    ...bucket,
    color: palette[index % palette.length],
  }));
});

const total = computed(() => slices.value.reduce((sum, slice) => sum + slice.count, 0) || 1);

const gradient = computed(() => {
  let start = 0;
  const stops = slices.value.map((slice) => {
    const end = start + (slice.count / total.value) * 360;
    const stop = `${slice.color} ${start}deg ${end}deg`;
    start = end;
    return stop;
  });
  return `conic-gradient(${stops.join(", ")})`;
});
</script>

<template>
  <section class="rounded-3xl border border-line bg-card p-5 sm:p-6">
    <h3 class="font-display text-2xl">Traffic</h3>
    <p class="text-sm text-muted">Click a link to load stats from the API.</p>

    <div v-if="links.statsLoading" class="mt-8 text-center text-muted">Loading stats…</div>

    <div v-else-if="links.selectedStats" class="mt-6 grid gap-6 md:grid-cols-[160px_1fr]">
      <div class="flex flex-col items-center">
        <div
          class="relative h-36 w-36 rounded-full"
          :style="{ background: gradient }"
          aria-hidden="true"
        >
          <div class="absolute inset-5 rounded-full bg-card"></div>
        </div>
        <p class="mt-3 font-display text-3xl">{{ links.selectedStats.clickCount }}</p>
        <p class="text-xs uppercase tracking-widest text-muted">total clicks</p>
      </div>

      <div>
        <p class="truncate text-sm text-muted">{{ links.selectedStats.originalUrl }}</p>
        <p class="mt-1 font-medium text-pine">/{{ links.selectedStats.shortCode }}</p>

        <ul class="mt-4 space-y-2">
          <li
            v-for="slice in slices"
            :key="slice.label"
            class="flex items-center justify-between gap-3 text-sm"
          >
            <span class="flex items-center gap-2">
              <span class="h-2.5 w-2.5 rounded-full" :style="{ background: slice.color }"></span>
              {{ slice.label || "direct" }}
            </span>
            <span class="text-muted">{{ slice.count }}</span>
          </li>
        </ul>

        <div v-if="links.selectedStats.byDay.length" class="mt-5">
          <p class="text-xs uppercase tracking-widest text-muted">Last days</p>
          <div class="mt-2 flex h-20 items-end gap-1">
            <div
              v-for="day in links.selectedStats.byDay"
              :key="day.label"
              class="flex-1 rounded-t bg-ember/80"
              :style="{
                height: `${Math.max(8, (day.count / Math.max(...links.selectedStats.byDay.map((d) => d.count), 1)) * 100)}%`,
              }"
              :title="`${day.label}: ${day.count}`"
            ></div>
          </div>
        </div>
      </div>
    </div>

    <p v-else class="mt-8 rounded-2xl bg-paper px-4 py-10 text-center text-muted">
      Stats will appear here once the backend implements
      <code class="text-ink">GET /api/url/stats/{shortCode}</code>.
    </p>
  </section>
</template>
