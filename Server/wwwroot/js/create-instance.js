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
    let response = await $.get("/api/fabric/versions")
    let items = "";
    Array.from(response).forEach(item => {
        items += `<div class="dropdown-item" value="${item.version}">${item.version}</div>`
    })
    modloaderVersionDropdown.find('.dropdown-items').html(items)
    modloaderVersionDropdown.find('.dropdown-item').each((_, item) => {
        target = $(item);
        target.on('click', e => {
            let target = $(e.currentTarget);
            let dropdown = target.parent().parent()

            dropdown.each((_, d) => {
                d = $(d)
                d.find('.dropdown-item.selected').removeClass('selected')
            })

            dropdown.find('.value').html(target.html())
            dropdown.attr('value', target.attr('value'))
            dropdown.blur()
            target.addClass('selected');
        })
    })
    modloaderVersionDropdown.find('.dropdown-items')[0].click();
}
async function loadForge() {
    modloaderVersionDropdown.find('.dropdown-items').html("")
    modloaderVersionCombo.css('display', "")
    modloaderVersionLabel.html('Forge Loader');
}