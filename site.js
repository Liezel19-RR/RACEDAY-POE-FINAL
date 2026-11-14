document.addEventListener("change", event => {
    const input = event.target;
    if (!(input instanceof HTMLInputElement) || !input.matches("[data-image-preview]")) return;
    const preview = input.parentElement.querySelector(".preview");
    if (!preview || !input.files || !input.files[0]) return;
    preview.src = URL.createObjectURL(input.files[0]);
    preview.hidden = false;
});

document.addEventListener("submit", event => {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) return;
    const button = form.querySelector("button[type=submit],button:not([type])");
    if (button && button.dataset.confirm && !window.confirm(button.dataset.confirm)) {
        event.preventDefault();
        return;
    }
    if (form.dataset.submitted === "true") {
        event.preventDefault();
        return;
    }
    form.dataset.submitted = "true";
    if (button) button.disabled = true;
});
