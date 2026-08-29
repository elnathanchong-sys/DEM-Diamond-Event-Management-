(function () {

    try {
        if (localStorage.getItem("dem-theme") === "light") {
            document.documentElement.setAttribute("data-theme", "light");
        } else {
            document.documentElement.removeAttribute("data-theme");
        }
    } catch (e) { }

    var STORAGE_KEY = "dem-theme";

    function getStoredTheme() {
        try {
            return localStorage.getItem(STORAGE_KEY) || "dark";
        } catch (e) {
            return "dark";
        }
    }

    function setStoredTheme(theme) {
        try {
            localStorage.setItem(STORAGE_KEY, theme);
        } catch (e) {
        }
    }

    function updateToggleUI(theme) {
        document.querySelectorAll(".theme-toggle").forEach(function (btn) {
            var icon = btn.querySelector("i");
            var label = btn.querySelector(".theme-toggle-label");
            if (icon) {
                icon.className = theme === "light" ? "bx bx-sun" : "bx bx-moon";
            }
            if (label) {
                label.textContent = theme === "light" ? "Light mode" : "Dark mode";
            }
        });
    }

    function applyTheme(theme) {
        if (theme === "light") {
            document.documentElement.setAttribute("data-theme", "light");
        } else {
            document.documentElement.removeAttribute("data-theme");
        }
        updateToggleUI(theme);
    }

    function toggleTheme() {
        var next = getStoredTheme() === "dark" ? "light" : "dark";
        setStoredTheme(next);
        applyTheme(next);
    }

    document.addEventListener("DOMContentLoaded", function () {
        applyTheme(getStoredTheme());
        document.querySelectorAll(".theme-toggle").forEach(function (btn) {
            btn.addEventListener("click", function (e) {
                e.preventDefault();
                toggleTheme();
            });
        });
    });
})();
