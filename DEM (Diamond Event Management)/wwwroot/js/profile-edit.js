document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("[data-image-preview]").forEach(function (input) {
        var preview = document.querySelector(input.dataset.imagePreview);
        var zone = input.closest("[data-file-drop-zone]");
        var message = zone?.querySelector("[data-file-validation]");
        var status = zone?.querySelector("[data-file-status]");
        var title = zone?.querySelector(".file-drop-title");
        var allowedTypes = ["image/jpeg", "image/png"];
        var maxSize = 5 * 1024 * 1024;

        if (!preview || !zone) return;

        function setMessage(text, isError) {
            if (!message) return;
            message.textContent = text;
            message.classList.toggle("is-visible", Boolean(text));
            message.classList.toggle("is-error", isError);
        }

        function handleFile(file) {
            var error = "";

            if (!file) return;

            var extension = file.name.split(".").pop().toLowerCase();
            if (!allowedTypes.includes(file.type) && !["jpg", "jpeg", "png"].includes(extension)) {
                error = "Only JPG, JPEG, or PNG images are allowed.";
            } else if (file.size > maxSize) {
                error = "Photo size cannot be more than 5 MB.";
            }

            if (error) {
                input.value = "";
                zone.classList.remove("is-selected");
                setMessage(error, true);
                if (status) status.textContent = "No new image selected";
                if (title) title.textContent = "Choose a profile image";
                return;
            }

            setMessage("", false);
            zone.classList.add("is-selected");
            if (status) status.textContent = "Selected: " + file.name;
            if (title) title.textContent = "Image ready to upload";

            var objectUrl = URL.createObjectURL(file);
            preview.src = objectUrl;
            preview.onload = function () {
                URL.revokeObjectURL(objectUrl);
            };
        }

        input.addEventListener("change", function () {
            handleFile(input.files && input.files[0]);
        });

        ["dragenter", "dragover"].forEach(function (eventName) {
            zone.addEventListener(eventName, function (event) {
                event.preventDefault();
                zone.classList.add("is-dragging");
            });
        });

        ["dragleave", "drop"].forEach(function (eventName) {
            zone.addEventListener(eventName, function (event) {
                event.preventDefault();
                zone.classList.remove("is-dragging");
            });
        });

        zone.addEventListener("drop", function (event) {
            var file = event.dataTransfer.files && event.dataTransfer.files[0];
            if (!file) return;

            try {
                var transfer = new DataTransfer();
                transfer.items.add(file);
                input.files = transfer.files;
            } catch {
                setMessage("Your browser could not attach the dropped file. Please use Browse.", true);
                return;
            }

            handleFile(file);
        });
    });
});
