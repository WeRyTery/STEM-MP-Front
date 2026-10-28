import ActivityChart from "../components/ActivityChart.jsx";

const sources = [
    { value: "GitHub", label: "GitHub" },
    { value: "StackOverflow", label: "Stack Overflow" },
    { value: "Himalayas", label: "Himalayas Jobs" }
];

const periods = [
    { value: "24h", label: "24 Hours" },
    { value: "7d", label: "7 Days" },
    { value: "30d", label: "30 Days" }
];

function formatNumber(value) {
    return Number(value).toLocaleString();
}

function formatPercent(value) {
    const number = Number(value);
    return `${number > 0 ? "+" : ""}${number.toFixed(2)}%`;
}

function FilterSelect({ label, value, options, onChange }) {
    return (
        <label className="filter-field">
            <span>{label}</span>
            <select value={value} onChange={event => onChange(event.target.value)}>
                {options.map(option => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                ))}
            </select>
        </label>
    );
}

export default function TechnologyDetails({
    technology,
    source,
    period,
    chartType,
    loading,
    error,
    onSourceChange,
    onPeriodChange,
    onChartTypeChange,
    onBack
}) {
    const history = technology?.history ?? [];
    const trend = technology?.trendPercent;
    const trendClass = trend == null ? "metric-value" : trend > 0
        ? "metric-value positive"
        : trend < 0 ? "metric-value negative" : "metric-value";

    return (
        <section className="details-view">
            <button className="back-button" type="button" onClick={onBack}>
                <span aria-hidden="true">←</span> Back to technologies
            </button>

            <header className="details-heading">
                <div>
                    <p className="eyebrow">TECHNOLOGY DETAILS</p>
                    <h1>{technology?.name ?? "Technology"}</h1>
                    <p className="details-description">Explore activity history and recent changes.</p>
                </div>
            </header>

            <section className="detail-filters" aria-label="Detail filters">
                <FilterSelect label="Data source" value={source} options={sources} onChange={onSourceChange} />
                <FilterSelect label="Time period" value={period} options={periods} onChange={onPeriodChange} />
                <FilterSelect
                    label="Chart type"
                    value={chartType}
                    options={[
                        { value: "line", label: "Line chart" },
                        { value: "bar", label: "Bar chart" },
                        { value: "doughnut", label: "Doughnut chart" }
                    ]}
                    onChange={onChartTypeChange}
                />
            </section>

            {error && <p className="message error-message" role="alert">{error}</p>}
            {loading && <p className="message">Loading technology details...</p>}

            <section className="metrics-grid" aria-label="Technology metrics">
                <article className="metric-card">
                    <span className="metric-label">Activity</span>
                    <strong className="metric-value">
                        {technology?.activityValue == null ? "Not enough data" : formatNumber(technology.activityValue)}
                    </strong>
                </article>
                <article className="metric-card">
                    <span className="metric-label">Change</span>
                    <strong className={trendClass}>
                        {trend == null ? "Not enough data" : formatPercent(trend)}
                    </strong>
                </article>
            </section>

            <section className="panel chart-panel" aria-labelledby="historyHeading">
                <header className="panel-heading">
                    <div>
                        <h2 id="historyHeading">Activity history</h2>
                        <p>Recorded values for the selected source and period.</p>
                    </div>
                </header>
                <div className="chart-container">
                    {!loading && <ActivityChart history={history} source={source} chartType={chartType} />}
                </div>
            </section>

            <section className="panel recommendation-panel" aria-labelledby="recommendationHeading">
                <header><h2 id="recommendationHeading">Recommendation</h2></header>
                <p>{technology?.recommendation ?? "Not enough data"}</p>
            </section>
        </section>
    );
}
