import { useEffect, useState } from "react";
import { getTechnologies, getTechnologyDetails } from "./api.js";
import TechnologyCard from "./components/TechnologyCard.jsx";
import TechnologyDetails from "./pages/TechnologyDetails.jsx";

const SOURCES = [
    { value: "GitHub", label: "GitHub" },
    { value: "StackOverflow", label: "Stack Overflow" },
    { value: "Himalayas", label: "Himalayas Jobs" }
];

const PERIODS = [
    { value: "24h", label: "24 Hours" },
    { value: "7d", label: "7 Days" },
    { value: "30d", label: "30 Days" }
];

export default function App() {
    // useState stores values that affect what React displays on the page
    const [search, setSearch] = useState("");
    const [category, setCategory] = useState("All");
    const [sort, setSort] = useState("name");
    const [source, setSource] = useState("GitHub");
    const [period, setPeriod] = useState("24h");
    const [chartType, setChartType] = useState("line");

    const [technologies, setTechnologies] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedId, setSelectedId] = useState(null);
    const [technologyDetails, setTechnologyDetails] = useState(null);
    const [listLoading, setListLoading] = useState(true);
    const [detailsLoading, setDetailsLoading] = useState(false);
    const [listError, setListError] = useState("");
    const [detailsError, setDetailsError] = useState("");

    // Load the available category names once. The backend remains unchanged
    useEffect(() => {
        const controller = new AbortController();

        getTechnologies({ search: "", category: "All", sort: "name", period: "24h", source: "GitHub" }, controller.signal)
            .then(items => {
                const names = [...new Set(items.map(item => item.category).filter(Boolean))].sort();
                setCategories(names);
            })
            .catch(error => {
                if (error.name !== "AbortError") {
                    console.warn("Could not load category options:", error);
                }
            });

        return () => controller.abort();
    }, []);

    // When search/filter settings change, fetch the list from the existing API
    useEffect(() => {
        if (selectedId !== null) return undefined;

        const controller = new AbortController();
        setListLoading(true);
        setListError("");

        getTechnologies({ search, category, sort, period, source }, controller.signal)
            .then(setTechnologies)
            .catch(error => {
                if (error.name !== "AbortError") {
                    setListError("Unable to load technologies. Check that the server is running.");
                }
            })
            .finally(() => {
                if (!controller.signal.aborted) setListLoading(false);
            });

        return () => controller.abort();
    }, [search, category, sort, period, source, selectedId]);

    // When a card is selected, load details. Changing source or period reloads them.
    useEffect(() => {
        if (selectedId === null) return undefined;

        const controller = new AbortController();
        setDetailsLoading(true);
        setDetailsError("");

        getTechnologyDetails(selectedId, { source, period }, controller.signal)
            .then(setTechnologyDetails)
            .catch(error => {
                if (error.name !== "AbortError") {
                    setDetailsError("Unable to load details. Check the server connection and try again.");
                }
            })
            .finally(() => {
                if (!controller.signal.aborted) setDetailsLoading(false);
            });

        return () => controller.abort();
    }, [selectedId, source, period]);

    function openTechnology(id) {
        setTechnologyDetails(null);
        setSelectedId(id);
    }

    function returnToList() {
        setSelectedId(null);
        setTechnologyDetails(null);
        // The list useEffect runs again and loads the current filters from the API
    }

    return (
        <div className="app-shell">
            <header className="site-header">
                <div className="container header-content">
                    <a className="logo" href="#top" onClick={event => { event.preventDefault(); returnToList(); }}>
                        <span className="logo-mark" aria-hidden="true">T</span>
                        TechTrend
                    </a>
                    <span className="header-subtitle">Technology trends dashboard</span>
                </div>
            </header>

            <main className="container main-content" id="top">
                {selectedId === null ? (
                    <section className="list-view">
                        <header className="page-heading">
                            <p className="eyebrow">DATA-DRIVEN OVERVIEW</p>
                            <h1>Technology trends</h1>
                            <p>Explore activity across programming technologies and data sources.</p>
                        </header>

                        <section className="filters" aria-label="Technology filters">
                            <label className="filter-field search-field">
                                <span>Search</span>
                                <input
                                    type="search"
                                    value={search}
                                    onChange={event => setSearch(event.target.value)}
                                    placeholder="Search technologies..."
                                />
                            </label>

                            <label className="filter-field">
                                <span>Category</span>
                                <select value={category} onChange={event => setCategory(event.target.value)}>
                                    <option value="All">All categories</option>
                                    {categories.map(name => <option key={name} value={name}>{name}</option>)}
                                </select>
                            </label>

                            <label className="filter-field">
                                <span>Sort by</span>
                                <select value={sort} onChange={event => setSort(event.target.value)}>
                                    <option value="name">Name</option>
                                    <option value="highest">Highest change</option>
                                    <option value="lowest">Lowest change</option>
                                    <option value="score">Highest activity</option>
                                </select>
                            </label>

                            <label className="filter-field">
                                <span>Data source</span>
                                <select value={source} onChange={event => setSource(event.target.value)}>
                                    {SOURCES.map(item => <option key={item.value} value={item.value}>{item.label}</option>)}
                                </select>
                            </label>
                        </section>

                        <section className="period-row" aria-label="Time period">
                            <span className="period-label">Time period</span>
                            <nav className="period-tabs" aria-label="Choose time period">
                                {PERIODS.map(item => (
                                    <button
                                        key={item.value}
                                        className={`period-tab ${period === item.value ? "active" : ""}`}
                                        type="button"
                                        aria-pressed={period === item.value}
                                        onClick={() => setPeriod(item.value)}
                                    >
                                        {item.label}
                                    </button>
                                ))}
                            </nav>
                        </section>

                        {listError && <p className="message error-message" role="alert">{listError}</p>}
                        {listLoading ? (
                            <p className="message" role="status">Loading technologies...</p>
                        ) : technologies.length === 0 ? (
                            <section className="empty-state">
                                <span className="empty-icon" aria-hidden="true">⌕</span>
                                <h2>No technologies found</h2>
                                <p>Try changing the search text or filters.</p>
                                <button className="secondary-button" type="button" onClick={() => {
                                    setSearch(""); setCategory("All"); setSort("name");
                                }}>Reset search and filters</button>
                            </section>
                        ) : (
                            <section className="technology-grid" aria-label="Technology list" aria-live="polite">
                                {technologies.map(technology => (
                                    <TechnologyCard
                                        key={technology.id}
                                        technology={technology}
                                        onOpen={() => openTechnology(technology.id)}
                                    />
                                ))}
                            </section>
                        )}
                    </section>
                ) : (
                    <TechnologyDetails
                        technology={technologyDetails}
                        source={source}
                        period={period}
                        chartType={chartType}
                        loading={detailsLoading}
                        error={detailsError}
                        onSourceChange={setSource}
                        onPeriodChange={setPeriod}
                        onChartTypeChange={setChartType}
                        onBack={returnToList}
                    />
                )}
            </main>

            <footer className="site-footer">
                <div className="container">
                    <p>TechTrend — technology activity dashboard</p>
                </div>
            </footer>
        </div>
    );
}
