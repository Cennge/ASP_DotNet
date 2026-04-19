class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    static jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    static jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

const errorBlock = document.getElementById("auth-error");

const showError = (msg) => {
    errorBlock.textContent = msg;
    errorBlock.classList.remove("d-none");
};

const hideError = () => {
    errorBlock.classList.add("d-none");
};

document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == 'auth-form') {
        e.preventDefault();
        hideError();
        const formData = new FormData(form);
        const login = formData.get("user-login");
        const password = formData.get("user-password");

        if (login.length == 0 && password.length == 0) {
            showError("Заповніть усі поля");
            return;
        }

        if (login.includes(':')) {
            showError("У логіні символ ':' не допускається");
            return;
        }

        const userPass = login + ':' + password;
        const basicCredentials = Base64.encode(userPass);
        fetch("/User/SignIn", {
            headers: {
                "Authorization": `Basic ${basicCredentials}`
            }
        })
        .then(r => r.json())
        .then(j => {
            if (j.status != 200) {
                showError("У вході відмовлено")
            }
            else {
                window.location.reload()
            }
        });
    }
})

function getFuncName() {
    return getFuncName.caller.name
}

document.addEventListener('DOMContentLoaded', () => {
    for (let btn of document.querySelectorAll("[data-add-to-cart]")) {
        btn.addEventListener('click', addToCartClick);
    }
    for (let btn of document.querySelectorAll("[data-inc-cart-itemid]")) {
        btn.addEventListener('click', incCartItemClick);
    }
    for (let btn of document.querySelectorAll("[data-delete-cart-itemid]")) {
        btn.addEventListener('click', deleteCartItemClick);
    }
});

function deleteCartItemClick(e) {
    if (!confirm("Підтверджуєте видалення товару з кошику?")) {
        return;
    }
    const attr = "data-delete-cart-itemid";
    const btn = e.target.closest(`[${attr}]`);
    if (!btn) {
        throw `${getFuncName()}: [${attr}] not found`;
    }
    const itemId = btn.getAttribute(attr);
    if (!itemId) {
        throw `${getFuncName()}: [${attr}] value is empty`;
    }
    console.log("delete", itemId);
    fetch(`/api/cart/${itemId}`, {
        method: 'DELETE'
    })
        .then(r => r.json())
        .then(j => {
            console.log(j);
            if (typeof j.data == 'object') {
                window.location.reload();
            }
        });
}
function incCartItemClick(e) {
    const attr = "data-inc-cart-itemid";
    const btn = e.target.closest(`[${attr}]`);
    if (!btn) {
        throw `${getFuncName()}: [${attr}] not found`;
    }
    const itemId = btn.getAttribute(attr);
    if (!itemId) {
        throw `${getFuncName()}: [${attr}] value is empty`;
    }
    const inc = btn.getAttribute("data-inc");
    if (!inc) {
        throw `${getFuncName()}: [data-inc] value is empty`;
    }
    console.log("dec", itemId);
    fetch(`/api/cart/${itemId}?inc=${inc}`, {
        method: 'PUT'
    })
        .then(r => r.json())
        .then(j => {
            console.log(j);
            if (typeof j.data == 'object') {
                window.location.reload();
            }
        });
}

function addToCartClick(e) {
    const btn = e.target.closest("[data-add-to-cart]")
    if (!btn) {
        throw "addToCartClick: [data-add-to-cart] not found"
    }
    const savedUser = window.localStorage.getItem("user-231");
    if (!savedUser) {
        alert("Для створення замовлення необхідно увійти до системи");
        return;
    }
    const productId = btn.getAttribute("data-add-to-cart")
    if (!productId) {
        throw "addToCartClick: [data-add-to-cart] value is empty"
    }
    console.log(btn.getAttribute("data-add-to-cart"))
    fetch("/api/cart/" + productId, {
        method: "POST"
    })
        .then(r => r.json())
        .then(j => {
            console.log(j)
            if (typeof j.data == 'object') {
                window.location.reload()
            }
        })
}