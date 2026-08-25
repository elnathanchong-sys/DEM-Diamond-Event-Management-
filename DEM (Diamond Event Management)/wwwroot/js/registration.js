// For registrationEvents filter toggle dropdown 
$(document).on('click', '#filterToggleBtn', function (e) {
    e.stopPropagation(); // stop this click from being treated as an "outside click" below
    $('#filterDropdown').toggleClass('open');
});

// Close the dropdown when the user clicks anywhere else on the page
$(document).on('click', function (e) {
    if (!$(e.target).closest('.filter-wrapper').length) {
        $('#filterDropdown').removeClass('open');
    }
});
