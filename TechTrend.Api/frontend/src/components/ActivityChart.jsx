import { useEffect, useRef } from "react";
import Chart from "chart.js/auto";

// Chart.js owns the canvas drawing. useEffect creates the chart when data changes
// and destroys the old instance before React re-renders it
export default function ActivityChart({ history, source, chartType }) {
    const canvasRef = useRef(null);

    useEffect(() => {
        if (!canvasRef.current || history.length === 0) {
            return undefined;
        }

        const labels = history.map(item =>
            new Date(item.collectedAt).toLocaleString("en-US", {
                month: "short",
                day: "numeric",
                hour: "2-digit",
                minute: "2-digit"
            })
        );
        const values = history.map(item => Number(item.activityValue ?? 0));
        const isCircular = chartType === "doughnut";

        const chart = new Chart(canvasRef.current, {
            type: chartType,
            data: {
                labels,
                datasets: [{
                    label: `${source} activity`,
                    data: values,
                    borderColor: "#5278db",
                    backgroundColor: isCircular
                        ? ["#5278db", "#35a27b", "#e4a23b", "#d76565", "#8b6bd6", "#46a8b5", "#9ca3af", "#e78ac3"]
                        : "rgba(82, 120, 219, 0.25)",
                    borderWidth: 2,
                    tension: 0.25,
                    fill: chartType === "line"
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: isCircular }
                },
                scales: isCircular ? {} : {
                    y: {
                        beginAtZero: true,
                        title: { display: true, text: "Activity" }
                    },
                    x: {
                        title: { display: true, text: "Collection date" }
                    }
                }
            }
        });

        // Cleanup is important: without destroy(), old charts can remain on the canvas
        return () => chart.destroy();
    }, [history, source, chartType]);

    if (history.length === 0) {
        return <p className="empty-chart">No history is available for this source and period.</p>;
    }

    return <canvas ref={canvasRef} aria-label="Technology activity history chart" role="img" />;
}
