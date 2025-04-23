$(() => {
    const contrib = new bootstrap.Modal($("new-contrib")[0]);
    const deposit = new bootstrap.Modal($("deposit")[0]);

    $("#new-Contributor").on('click', function () {
        $("#fn").val('');
        $("#ln").val('');
        $("#cell").val('');
        $("#date").val('');
        $("#always_include").prop('checked', false);
        $("#edit-id").remove();
        $(".new-conrtib form").attr("action", "/contributors/new"); 
        $(".new-contrib h5").text('New Contributor');
        $("#initial").show();

        contrib.show();
    })

    $(".deposit-button").on('click', function () {
        const name = $(this).data('contribName');
        $("#deposit-name").text(name);
        const id = $(this).data.('contribId');
        $('#hidden').val(id);

        deposit.show();

    })
     
    $(".edit-contrib").on('click', function () {
        $(".new-contrib h5").text('Edit Contributor');
        $("#initial").hide();
        $("#fn").val($(this).data('first-name'));
        $("#ln").val($(this).data('last-name'));
        $("#cell").val($(this).data('cell'));
        $("#date").val($(this).data('date'));
        const alwaysInclude = $(this).data('always-include') === 'True';
        $("#always_include").prop('checked', alwaysInclude);

        $("#edit-id").remove();
        $(".new-contrib form").append(`<input type="hidden" id="edit-id" name="id" value=${$(this).data('id')}>`)
        $(".new-contrib form").attr("action", "/contributors/edit");

        contrib.show();
    })

    $("#search").on('input', function () {
        const searchText = $(this).val().toLowerCase();

        $(".person-row").each(function () {
            const row = $(this);
            const name = row.find('.name-cell').text().toLowerCase();

            if (name.includes(searchText)) {
                row.show();
            } else {
                row.hide();
            }
        })
    })

    $("#clear").on('click', function () {
        $("#search").val('');
        $(".person-row").show();
    })
});