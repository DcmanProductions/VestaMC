let options = {
    instanceName: $("#instance-name").val(),
    javaVersion: $("#java-dropdown").attr('value'),
    minecraftVersion: $("#minecraft-version-dropdown").attr('value'),
    modloader: $("#modloader-dropdown").attr('value'),
    modloaderVersion: "",
    javaArchive: null,
}

$("#instance-name").on('keyup', e => options.instanceName = $(e.currentTarget).val())
$("#modloader-dropdown").on('change', async e => await loadModded($(e.currentTarget).attr('value')))
$("#minecraft-version-dropdown").on('change', e => {
    options.minecraftVersion = $(e.currentTarget).attr('value');
    if (options.modloader == "forge")
        loadModded(options.modloader);
})
$("#java-dropdown").on('change', e => {
    let version = $(e.currentTarget).attr('value');
    if (version == "custom") {
        let input = document.createElement("input");
        input.type = "file";
        input.accept = "application/zip, application/gzip, .tar.gz"
        input.addEventListener('change', () => {
            if (input.files.length != 0) {
                javaArchive = input.files[0];
            }
        })
        input.click();
    } else {
        options.javaVersion = version
    }
})

async function loadModded(loader) {
    options.modloader = loader;
    let modloaderVersionCombo = $("#modloader-version-dropdown, label[for='modloader-version-dropdown']");
    // If the loader is vanilla ignore it.
    if (loader == "vanilla") {
        modloaderVersionCombo.css('display', 'none');

        return;
    }

    // Set the dropdown and label to be visible
    modloaderVersionCombo.css('display', "")
    $("label[for='modloader-version-dropdown']").html(`${loader} loader`); // Set the label text

    // Make the call to the api
    let response = await $.get(`/api/${loader}/versions${(loader == "forge" ? `?mc=${options.minecraftVersion}` : "")}`)

    // Loop through all json items and create html dropdown item.
    let items = "";
    Array.from(response).forEach(item => {
        items += `<div class="dropdown-item" value="${item.version}">${item.version}</div>`
    })

    let modloaderVersionDropdown = $("#modloader-version-dropdown");

    // Insert html into the dropdown items
    modloaderVersionDropdown.find('.dropdown-items').html(items)

    // Regsister the click event for the dropdown items
    modloaderVersionDropdown.find('.dropdown-item').on('click', e => {
        RegisterDropdownItemClickEvent(e.currentTarget)
        options.modloaderVersion = $(e.currentTarget).attr('value')
    })
    modloaderVersionDropdown.find('.dropdown-item')[0].click()
}