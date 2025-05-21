document.addEventListener('DOMContentLoaded', () => {
    const burger = document.querySelector('.burger');
    const navList = document.querySelector('.nav__list');

    burger.addEventListener('click', () => {
        const isOpen = burger.getAttribute('aria-expanded') === 'true';
        burger.setAttribute('aria-expanded', String(!isOpen));
        navList.classList.toggle('active');
        burger.innerHTML = isOpen ? '&#9776;' : '&times;';
    });

    document.querySelectorAll('.nav__list a').forEach(link => {
        link.addEventListener('click', () => {
            burger.setAttribute('aria-expanded', 'false');
            navList.classList.remove('active');
            burger.innerHTML = '&#9776;';
        });
    });
});
document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('contact-form');

    form.addEventListener('submit', async e => {
        e.preventDefault();
        const data = Object.fromEntries(new FormData(form));

        try {
            const res = await fetch('/api/contact', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (!res.ok) throw new Error('Error sending message');

            alert('Message sent successfully');
            form.reset();
        } catch (err) {
            alert('Something went wrong');
        }
    });
});

