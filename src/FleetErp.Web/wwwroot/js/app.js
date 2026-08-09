// Comportamientos del portal. Sin marcos de terceros: lo que hace falta cabe en
// unas cuantas líneas y así el portal carga igual de rápido en la laptop del
// despachador que en la oficina.
(function () {
    "use strict";

    // Menú lateral en pantallas chicas.
    var toggle = document.querySelector("[data-rail-toggle]");
    var nav = document.querySelector("[data-rail-nav]");
    if (toggle && nav) {
        toggle.addEventListener("click", function () {
            var open = nav.classList.toggle("is-open");
            toggle.setAttribute("aria-expanded", String(open));
        });
    }

    // Los filtros se aplican al cambiar el control: nadie debería tener que
    // buscar un botón "Filtrar" después de elegir un estado.
    document.querySelectorAll("[data-autosubmit]").forEach(function (control) {
        control.addEventListener("change", function () {
            var form = control.closest("form");
            if (form) form.submit();
        });
    });

    // Confirmación de acciones que no se pueden deshacer.
    document.querySelectorAll("[data-confirm]").forEach(function (element) {
        element.addEventListener("click", function (event) {
            if (!window.confirm(element.getAttribute("data-confirm"))) event.preventDefault();
        });
    });

    // Paleta compartida por todas las gráficas, tomada del color de la empresa.
    var css = getComputedStyle(document.documentElement);
    var palette = {
        brand: css.getPropertyValue("--brand").trim() || "#0E7C66",
        signal: css.getPropertyValue("--signal").trim() || "#C8860D",
        alert: css.getPropertyValue("--alert").trim() || "#A3271C",
        ink: css.getPropertyValue("--ink-soft").trim() || "#545C63",
        rule: css.getPropertyValue("--rule").trim() || "#D8DBD4"
    };

    function withAlpha(hex, alpha) {
        var value = hex.replace("#", "");
        if (value.length !== 6) return hex;
        var r = parseInt(value.slice(0, 2), 16);
        var g = parseInt(value.slice(2, 4), 16);
        var b = parseInt(value.slice(4, 6), 16);
        return "rgba(" + r + "," + g + "," + b + "," + alpha + ")";
    }

    if (typeof Chart === "undefined") return;

    Chart.defaults.font.family = css.getPropertyValue("--body").trim() || "Segoe UI, sans-serif";
    Chart.defaults.font.size = 11;
    Chart.defaults.color = palette.ink;
    Chart.defaults.plugins.legend.labels.boxWidth = 10;
    Chart.defaults.plugins.legend.labels.usePointStyle = true;
    Chart.defaults.maintainAspectRatio = false;
    Chart.defaults.animation = window.matchMedia("(prefers-reduced-motion: reduce)").matches
        ? false
        : { duration: 420 };

    var currency = document.body.getAttribute("data-currency") || "$";

    function formatCompact(value) {
        var abs = Math.abs(value);
        if (abs >= 1000000) return (value / 1000000).toFixed(1) + " M";
        if (abs >= 1000) return (value / 1000).toFixed(0) + " k";
        return String(Math.round(value));
    }

    var axes = {
        x: { grid: { display: false }, ticks: { maxRotation: 0, autoSkipPadding: 14 } },
        y: {
            beginAtZero: true,
            border: { display: false },
            grid: { color: palette.rule },
            ticks: { callback: formatCompact }
        }
    };

    // Cada gráfica se declara con atributos en el HTML; este bloque solo las
    // instancia, de modo que las vistas no llevan JavaScript embebido.
    document.querySelectorAll("canvas[data-chart]").forEach(function (canvas) {
        var kind = canvas.getAttribute("data-chart");
        var labels = JSON.parse(canvas.getAttribute("data-labels") || "[]");
        var values = JSON.parse(canvas.getAttribute("data-values") || "[]");
        var label = canvas.getAttribute("data-label") || "";
        var money = canvas.getAttribute("data-money") === "true";

        var tooltip = {
            callbacks: {
                label: function (context) {
                    var raw = context.parsed.y !== undefined && context.parsed.y !== null
                        ? context.parsed.y
                        : context.parsed;
                    var text = raw.toLocaleString("es-MX", { maximumFractionDigits: 2 });
                    return " " + (money ? currency + text : text);
                }
            }
        };

        if (kind === "line" || kind === "area") {
            new Chart(canvas, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                        label: label,
                        data: values,
                        borderColor: palette.brand,
                        backgroundColor: withAlpha(palette.brand, .12),
                        borderWidth: 2,
                        fill: kind === "area",
                        tension: .3,
                        pointRadius: 0,
                        pointHoverRadius: 4
                    }]
                },
                options: { scales: axes, plugins: { legend: { display: false }, tooltip: tooltip } }
            });
            return;
        }

        if (kind === "bars") {
            new Chart(canvas, {
                type: "bar",
                data: {
                    labels: labels,
                    datasets: [{
                        label: label,
                        data: values,
                        backgroundColor: values.map(function (v) {
                            return v < 0 ? palette.alert : palette.brand;
                        }),
                        borderRadius: 3,
                        maxBarThickness: 26
                    }]
                },
                options: { scales: axes, plugins: { legend: { display: false }, tooltip: tooltip } }
            });
            return;
        }

        if (kind === "hbars") {
            new Chart(canvas, {
                type: "bar",
                data: {
                    labels: labels,
                    datasets: [{
                        label: label,
                        data: values,
                        backgroundColor: withAlpha(palette.brand, .85),
                        borderRadius: 3,
                        maxBarThickness: 20
                    }]
                },
                options: {
                    indexAxis: "y",
                    scales: {
                        x: { beginAtZero: true, border: { display: false }, grid: { color: palette.rule }, ticks: { callback: formatCompact } },
                        y: { grid: { display: false } }
                    },
                    plugins: { legend: { display: false }, tooltip: tooltip }
                }
            });
            return;
        }

        if (kind === "donut") {
            var wedges = [palette.brand, palette.signal, palette.alert, "#4A6E93", "#7E6BA8", "#5B8C6E", "#9C7A4B"];
            new Chart(canvas, {
                type: "doughnut",
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: labels.map(function (_, i) { return wedges[i % wedges.length]; }),
                        borderWidth: 2,
                        borderColor: "#fff"
                    }]
                },
                options: {
                    cutout: "62%",
                    plugins: {
                        legend: { position: "right" },
                        tooltip: tooltip
                    }
                }
            });
        }
    });

    // Comparativa de dos series (vendido contra costo) en la pantalla de finanzas.
    document.querySelectorAll("canvas[data-chart-dual]").forEach(function (canvas) {
        var labels = JSON.parse(canvas.getAttribute("data-labels") || "[]");
        var a = JSON.parse(canvas.getAttribute("data-values-a") || "[]");
        var b = JSON.parse(canvas.getAttribute("data-values-b") || "[]");

        new Chart(canvas, {
            type: "line",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: canvas.getAttribute("data-label-a") || "Serie A",
                        data: a,
                        borderColor: palette.brand,
                        backgroundColor: withAlpha(palette.brand, .12),
                        borderWidth: 2, fill: true, tension: .3, pointRadius: 0
                    },
                    {
                        label: canvas.getAttribute("data-label-b") || "Serie B",
                        data: b,
                        borderColor: palette.signal,
                        backgroundColor: withAlpha(palette.signal, .1),
                        borderWidth: 2, fill: true, tension: .3, pointRadius: 0
                    }
                ]
            },
            options: { scales: axes, plugins: { legend: { position: "top", align: "end" } } }
        });
    });
})();
