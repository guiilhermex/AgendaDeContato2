const CONTATO_API = "/Contato";

let contatoExcluirId = null;

/* =========================
   LISTAR / FILTRAR CONTATOS
========================= */
async function carregarContatos(pageNumber = 1, pageSize = 10) {
    const nome = document.getElementById("FiltroContatoNome")?.value ?? "";

    try {
        const response = await fetch(
            `${CONTATO_API}?nome=${encodeURIComponent(nome)}&pageNumber=${pageNumber}&pageSize=${pageSize}`
        );

        if (response.status === 401) {
            mostrarToast("Usuário não autorizado", "danger");
            return;
        }

        if (!response.ok) {
            mostrarToast("Erro ao carregar contatos", "danger");
            return;
        }

        const result = await response.json();
        const contatos = result.data.data;

        const tbody = document.getElementById("dadosTabela");
        tbody.innerHTML = "";

        contatos.forEach(contato => {
            tbody.innerHTML += `
                <tr>
                    <td>${contato.nomeContato}</td>
                    <td>${contato.email}</td>
                    <td>-</td>
                    <td>
                        <button class="btn btn-sm btn-warning me-1"
                            onclick="editarContato(${contato.idContato})">
                            Editar
                        </button>
                        <button class="btn btn-sm btn-danger"
                            onclick="confirmarExcluirContato(${contato.idContato})">
                            Excluir
                        </button>
                    </td>
                </tr>
            `;
        });

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado ao carregar contatos", "danger");
    }
}

/* =========================
   FILTRO
========================= */
document.getElementById("btnFiltrarContato")
    ?.addEventListener("click", () => carregarContatos());

/* =========================
   BUSCAR POR ID
========================= */
async function editarContato(id) {
    try {
        const response = await fetch(`${CONTATO_API}/${id}`);

        if (!response.ok) {
            mostrarToast("Contato não encontrado", "danger");
            return;
        }

        const result = await response.json();
        const contato = result.data;

        document.getElementById("ContatoId").value = contato.idContato;
        document.getElementById("ContatoNome").value = contato.nomeContato;
        document.getElementById("ContatoEmail").value = contato.email;
        document.getElementById("ContatoTelefone").value = contato.telefone ?? "";

        document.getElementById("tituloModalContato").innerText = "Editar Contato";

        new bootstrap.Modal(
            document.getElementById("modalContato")
        ).show();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro ao buscar contato", "danger");
    }
}

/* =========================
   SALVAR (CRIAR / EDITAR)
========================= */
document.getElementById("btnSalvarContato")
    ?.addEventListener("click", salvarContato);

async function salvarContato() {
    const id = document.getElementById("ContatoId").value;
    const nomeContato = document.getElementById("ContatoNome").value.trim();
    const email = document.getElementById("ContatoEmail").value.trim();
    const telefone = document.getElementById("ContatoTelefone").value.trim();

    if (!nomeContato || !email) {
        mostrarToast("Preencha os campos obrigatórios", "warning");
        return;
    }

    const payload = {
        NomeContato: nomeContato,
        Email: email,
        Telefone: telefone
    };

    const url = id
        ? `${CONTATO_API}/Editar/${id}`
        : `${CONTATO_API}/Criar`;

    const method = id ? "PUT" : "POST";

    try {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            mostrarToast("Erro ao salvar contato", "danger");
            return;
        }

        mostrarToast(
            id ? "Contato atualizado com sucesso!" : "Contato criado com sucesso!",
            "success"
        );

        bootstrap.Modal.getInstance(
            document.getElementById("modalContato")
        ).hide();

        document.getElementById("formContato").reset();
        document.getElementById("ContatoId").value = "";

        carregarContatos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}

/* =========================
   EXCLUSÃO
========================= */
function confirmarExcluirContato(id) {
    contatoExcluirId = id;

    new bootstrap.Modal(
        document.getElementById("modalConfirmacao")
    ).show();
}

document.getElementById("btnConfirmarExclusao")
    ?.addEventListener("click", excluirContato);

async function excluirContato() {
    if (!contatoExcluirId) return;

    try {
        const response = await fetch(
            `${CONTATO_API}/Deletar/${contatoExcluirId}`,
            { method: "DELETE" }
        );

        if (!response.ok) {
            mostrarToast("Erro ao excluir contato", "danger");
            return;
        }

        mostrarToast("Contato excluído com sucesso!", "success");
        carregarContatos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}
