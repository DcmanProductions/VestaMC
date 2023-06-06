$(".dropdown-item").on('click', e => {
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
$(".dropdown").each((_, item) => {
    item = $(item)
    let selected = item.find(`.dropdown-item[value='${item.attr('value')}']`)
    selected.addClass('selected');
    item.find(`.value`).html(selected.html());
})