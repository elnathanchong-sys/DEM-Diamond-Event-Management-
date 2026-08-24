// ==========================================
// JQUERY UTILITIES
// ==========================================

$(document).on('click', '[data-get]', e => {
    e.preventDefault();
    const url = e.target.dataset.get;
    location = url || location;
});

$(document).on('click', '[data-post]', e => {
    e.preventDefault();
    const url = e.target.dataset.post;
    const f = $('<form>').appendTo(document.body)[0];
    f.method = 'post';
    f.action = url || location;
    f.submit();
});

$('[data-trim]').on('change', e => {
    e.target.value = e.target.value.trim();
});

$('[data-upper]').on('input', e => {
    const a = e.target.selectionStart;
    const b = e.target.selectionEnd;
    e.target.value = e.target.value.toUpperCase();
    e.target.setSelectionRange(a, b);
});

$('[type=reset]').on('click', e => {
    e.preventDefault();
    location = location;
});

$('[data-check]').on('click', e => {
    e.preventDefault();
    const name = e.target.dataset.check;
    $(`[name=${name}]`).prop('checked', true);
});

$('[data-uncheck]').on('click', e => {
    e.preventDefault();
    const name = e.target.dataset.uncheck;
    $(`[name=${name}]`).prop('checked', false);
});

$(document).on('click', '[data-checkable]', e => {
    if ($(e.target).is(':input,a')) return;
    $(e.currentTarget).find(':checkbox').prop('checked', (i, v) => !v);
});

$(document).on('change', '.upload input[type=file]', e => {
    const f = e.target.files[0];
    const img = $(e.target).siblings('img')[0];
    if (!img) return;

    img.dataset.src ??= img.src;

    if (f && f.type.startsWith('image/')) {
        img.onload = e => URL.revokeObjectURL(img.src);
        img.src = URL.createObjectURL(f);

        $(`[data-valmsg-for="${e.target.name}"]`)
            .text('')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
    } else {
        img.src = img.dataset.src;
        e.target.value = '';
    }

    $(e.target).valid();
});

// Index image slider
document.addEventListener("DOMContentLoaded", function () {
    const slider = document.querySelector('.banner-img-carousel-slider');
    const sliderImages = document.querySelectorAll('.banner-img-carousel-slider img');
    const next = document.querySelector('.next');
    const prev = document.querySelector('.prev');
    const dots = document.querySelectorAll('.banner-img-carousel-nav a');
    const sliderWrapper = document.querySelector('.banner-img-carousel-wrapper');

    if (!slider || sliderImages.length === 0) return;

    let counter = 0;
    let autoSlideTimer; // Variable to store the interval timer reference

    function goToSlide(index) {
        counter = index;
        const slideWidth = slider.clientWidth;

        // Scroll container horizontally
        slider.scrollTo({
            left: slideWidth * counter,
            behavior: 'smooth'
        });

        updateDots();
    }

    function updateDots() {
        dots.forEach((dot, idx) => {
            dot.classList.toggle('active', idx === counter);
        });
    }

    // starts or resets the 5 second automatic sliding
    function startAutoSlide() {
        clearInterval(autoSlideTimer);
        autoSlideTimer = setInterval(() => {
            let nextIndex = (counter + 1) % sliderImages.length;
            goToSlide(nextIndex);
        }, 5000); // 5000ms = 5 seconds
    }

    // start auto-sliding when the page loads
    startAutoSlide();

    // pause auto-slide on hover, resume when mouse leaves
    if (sliderWrapper) {
        sliderWrapper.addEventListener('mouseenter', () => clearInterval(autoSlideTimer));
        sliderWrapper.addEventListener('mouseleave', startAutoSlide);
    }

    // next Button Click
    if (next) {
        next.addEventListener('click', () => {
            let nextIndex = (counter + 1) % sliderImages.length;
            goToSlide(nextIndex);
            startAutoSlide(); // Reset timer so it waits a full 5s after a click
        });
    }

    // Prev Button Click
    if (prev) {
        prev.addEventListener('click', () => {
            let prevIndex = (counter - 1 + sliderImages.length) % sliderImages.length;
            goToSlide(prevIndex);
            startAutoSlide(); // Reset timer
        });
    }

    // Dot Clicks
    dots.forEach((dot, index) => {
        dot.addEventListener('click', (e) => {
            e.preventDefault();
            goToSlide(index);
            startAutoSlide(); // Reset timer
        });
    });
});

//hover/change animation for index events status
$(document).on('click', '.status-tab', function () {
    // Remove status active class from all tabs
    $('.status-tab').removeClass('active');

    // Add status active class to the clicked tab
    $(this).addClass('active');
});

//index page evetn status slider
$(document).on('click', '.event-carousel-next', function () {
    var slider = $(this).siblings('.event-carousel-slider')[0];
    if (!slider) return;
    slider.scrollBy({ left: slider.clientWidth * 0.8, behavior: 'smooth' });
});

$(document).on('click', '.event-carousel-prev', function () {
    var slider = $(this).siblings('.event-carousel-slider')[0];
    if (!slider) return;
    slider.scrollBy({ left: -slider.clientWidth * 0.8, behavior: 'smooth' });
});

// Event list collapse function
$(document).on('click', '.collapse-all-link', function (e) {
    e.preventDefault();

    var $link = $(this);
    var isCollapsing = $link.data('state') !== 'collapsed';

    $('.filter-group').prop('open', !isCollapsing);
    $link.text(isCollapsing ? 'Expand All' : 'Collapse All');
    $link.data('state', isCollapsing ? 'collapsed' : 'expanded');
});

// Automatically submit the filter form whenever any checkbox state changes this is inside eventlist
$(document).on('change', '#filter-form input[type="checkbox"]', function () {
    $(this).closest('form').submit();
});

