$(".dropdown-item").on('click', e => RegisterDropdownItemClickEvent(e.currentTarget))
$(".dropdown").each((_, item) => {
    item = $(item)
    let selected = item.find(`.dropdown-item[value='${item.attr('value')}']`)
    selected.addClass('selected');
    item.find(`.value`).html(selected.html());
})

function RegisterDropdownItemClickEvent(dropdownItem) {
    dropdownItem = $(dropdownItem)
    let dropdown = dropdownItem.parent().parent()

    dropdown.each((_, d) => {
        d = $(d)
        d.find('.dropdown-item.selected').removeClass('selected')
    })

    dropdown.find('.value').html(dropdownItem.html())
    dropdown.attr('value', dropdownItem.attr('value'))
    dropdown.blur()
    dropdownItem.addClass('selected');
}