document.addEventListener("DOMContentLoaded", function () {
    const modalOverlay = document.getElementById("registerModalOverlay");
    const openBtn = document.getElementById("openRegisterModalBtn");
    const closeBtn = document.getElementById("closeRegisterModalBtn");

    // ----------------Modal open and close----------------
    if (modalOverlay) {
        // Modal = pop up window overlay
        if (openBtn) {
            openBtn.addEventListener("click", function () {
                modalOverlay.classList.add("open");
            });
        }

        // Close modal with close button
        if (closeBtn) {
            closeBtn.addEventListener("click", function () {
                modalOverlay.classList.remove("open");
            });
        }

        // Close modal when clicking the dark backdrop outside the card
        modalOverlay.addEventListener("click", function (e) {
            if (e.target === modalOverlay) {
                modalOverlay.classList.remove("open");
            }
        });
    }

    const container = document.getElementById("playerFieldsContainer");
    const addBtn = document.getElementById("addPlayerBtn");
    if (!container || !addBtn) return;

    const maxPlayers = parseInt(container.getAttribute("data-max-players")) || 7;

    addBtn.addEventListener("click", function () {
        const currentCount = container.querySelectorAll(".event-individual-player-box").length;

        //for when they add players/enter player igns
        if (currentCount < maxPlayers) {
            const nextIndex = currentCount + 1;

            let roleLabel = `Player ${nextIndex} IGN`;
            if (nextIndex === 6) roleLabel = "Substitute IGN";
            else if (nextIndex === 7) roleLabel = "Coach IGN";
            else if (nextIndex > 7) roleLabel = `Extra Member ${nextIndex} IGN`;

            const div = document.createElement("div");
            div.className = "event-individual-input-box event-individual-player-box";
            div.innerHTML = `
                <label>${roleLabel}</label>
                <input type="text" name="PlayerIGNs" placeholder="Enter handle..." />
            `;
            container.appendChild(div);

            if (currentCount + 1 >= maxPlayers) {
                addBtn.style.display = "none";
            }
        }
    });

    const alerts = document.querySelectorAll(".event-individual-alert");
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.add("fade-out");
            setTimeout(function () {
                alert.remove();
            }, 500);
        }, 4000); //pop up window for 4 seconds before fading
    });
});