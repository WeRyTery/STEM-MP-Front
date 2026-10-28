// All requests go through the existing ASP.NET Core API
const API_URL = "/api/technologies";

function buildQuery({ search = "", category = "All", sort = "name", period = "24h", source = "GitHub" }) {
    // URLSearchParams safely encodes values such as spaces in the search text.
    return new URLSearchParams({ search, category, sort, period, source }).toString();
}

async function readJson(response) {
    if (!response.ok) {
        // Shows a simple status instead of stack traces to user
        throw new Error(`Request failed (HTTP ${response.status})`);
    }

    return response.json();
}

export async function getTechnologies(filters, signal) {
    const query = buildQuery(filters);
    const response = await fetch(`${API_URL}?${query}`, { signal });
    return readJson(response);
}

export async function getTechnologyDetails(id, filters, signal) {
    const query = buildQuery({ ...filters, search: "", category: "All", sort: "name" });
    const response = await fetch(`${API_URL}/${id}?${query}`, { signal });
    return readJson(response);
}
