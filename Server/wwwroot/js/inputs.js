$(".dropdown-item").on('click', e => {
    let target = $(e.currentTarget);
    target.addClass('selected');
    let dropdown = target.parent().parent()
    dropdown.find('.value').html(target.html())
    dropdown.attr('value', target.attr('value'))
    dropdown.blur()
})
$(".dropdown").each((_, item) => {
    item = $(item)
    let selected = item.find(`.dropdown-item[value='${item.attr('value')}']`)
    selected.addClass('selected');
    item.find(`.value`).html(selected.html());
})
