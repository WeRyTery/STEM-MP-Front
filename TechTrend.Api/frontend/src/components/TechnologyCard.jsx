function formatNumber(value) {
    return Number(value).toLocaleString();
}

function formatPercent(value) {
    const number = Number(value);
    return `${number > 0 ? "+" : ""}${number.toFixed(2)}%`;
}

// One card for one technology. The parent supplies its data and click handler
export default function TechnologyCard({ technology, onOpen }) {
    const hasChange = technology.changePercent != null;
    const changeClass = !hasChange
        ? "trend-value"
        : technology.changePercent > 0
            ? "trend-value positive"
            : technology.changePercent < 0
                ? "trend-value negative"
                : "trend-value";

    return (
        <button className="tech-card" type="button" onClick={onOpen}>
            <span className="card-topline">
                <span className="category-badge">{technology.category || "Other"}</span>
                <span className="card-arrow" aria-hidden="true">↗</span>
            </span>
            <h2>{technology.name}</h2>
            <span className="card-metric-label">Activity</span>
            <strong className="card-activity">
                {technology.activityValue == null
                    ? "Not enough data"
                    : formatNumber(technology.activityValue)}
            </strong>
            <span className="card-change-row">
                <span>Change</span>
                <strong className={changeClass}>
                    {hasChange ? formatPercent(technology.changePercent) : "Not enough data"}
                </strong>
            </span>
            <span className="card-open-hint">View activity history</span>
        </button>
    );
}
