window.dropdownHelper = {
    toggle: (buttonId) => {
        const button = document.getElementById(buttonId);
        if (button) {
            const bsDropdown = new bootstrap.Dropdown(button);
            bsDropdown.toggle();
        }
    },
    close: (buttonId) => {
        const button = document.getElementById(buttonId);
        if (button) {
            const bsDropdown = bootstrap.Dropdown.getInstance(button);
            if (bsDropdown) {
                bsDropdown.hide();
            }
        }
    }
};
