// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let username = '';

function nextStep() {
    let emailStep = document.getElementById("step-email");
    let passwordStep = document.getElementById("step-password");
    username = document.getElementById("username").value;

    emailStep.classList.add('visually-hidden');
    passwordStep.classList.remove('visually-hidden')
}

function signIn(domain) {
    let loader = document.getElementById("loader");
    loader.classList.remove('visually-hidden');
    let password = document.getElementById("password").value;
    let url = `https://localhost:7265/${domain}/sigh-in`;
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username: username, password: password })
    })
    .then((response) => {
        loader.classList.add('visually-hidden');
        if (response.ok) {
            return response.json();
        }
        return Promise.reject(response);
    })
        .then((response) => {
            alert('Sign in status: ' + response.result);
            location.assign(response.redirectUrl);
        })
    .catch((response) => {
        console.log(response.status, response.statusText);
        // 3. get error messages, if any
        response.json().then((json) => {
            console.error(json.error);
            alert(json.error);
        });
    });
}