let modloaderVersionCombo = $("#modloader-version-dropdown, label[for='modloader-version-dropdown']")
let modloaderVersionLabel = $("label[for='modloader-version-dropdown']")
modloaderVersionCombo.css('display', 'none')
let modloaderVersionDropdown = $("#modloader-version-dropdown")
let modloaderDropdown = $("#modloader-dropdown")
modloaderDropdown.find(".dropdown-item[value='fabric']").on('click', async () => await loadFabric())
modloaderDropdown.find(".dropdown-item[value='forge']").on('click', async () => await loadForge())
modloaderDropdown.find(".dropdown-item[value='vanilla']").on('click', () => modloaderVersionCombo.css('display', 'none'))


async function loadFabric() {
    modloaderVersionCombo.css('display', "")
    modloaderVersionLabel.html('Fabric Loader');
}
async function loadForge() {
    modloaderVersionCombo.css('display', "")
    modloaderVersionLabel.html('Forge Loader');
}
