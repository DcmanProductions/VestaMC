let modloaderVersionCombo = $("#modloader-version-dropdown, label[for='modloader-version-dropdown']");
let modloaderVersionLabel = $("label[for='modloader-version-dropdown']");
modloaderVersionCombo.css('display', 'none');
let modloaderVersionDropdown = $("#modloader-version-dropdown");
let modloaderDropdown = $("#modloader-dropdown");
modloaderDropdown.find(".dropdown-item").on('click', async e => await loadModded($(e.currentTarget).attr('value')));
modloaderDropdown.find(".dropdown-item[value='vanilla']").on('click', () => modloaderVersionCombo.css('display', 'none'));

async function loadModded(loader) {
    // If the loader is vanilla ignore it.
    if (loader == "vanilla") return;

    // Set the dropdown and label to be visible
    modloaderVersionCombo.css('display', "")
    modloaderVersionLabel.html(`${loader} loader`); // Set the label text

    // Make the call to the api
    let response = await $.get(`/api/${loader}/versions`)

    // Loop through all json items and create html dropdown item.
    let items = "";
    Array.from(response).forEach(item => {
        items += `<div class="dropdown-item" value="${item.version}">${item.version}</div>`
    })

    // Insert html into the dropdown items
    modloaderVersionDropdown.find('.dropdown-items').html(items)

    // Regsister the click event for the dropdown items
    modloaderVersionDropdown.find('.dropdown-item').on('click', e => RegisterDropdownItemClickEvent(e.currentTarget))
    modloaderVersionDropdown.find('.dropdown-item')[0].click()
}