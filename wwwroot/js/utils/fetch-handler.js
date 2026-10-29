export async function fetchHandler(handler, params = {}) {
  const query = new URLSearchParams({ handler });
  Object.entries(params).forEach(([k, v]) => query.append(k, v));
  const response = await fetch(`?${query}`);
  if (!response.ok) throw new Error(`Handler ${handler} failed`);
  return response.json();
}
