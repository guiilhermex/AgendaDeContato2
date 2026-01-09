const GRUPO_API = "/Grupo";

let grupoExcluirId = null;

document.addEventListener("DOMContentLoaded", () => {
    carregarGrupos();

    document
        .getElementById("btnSalvarGrupo")
        .addEventListener("click", salvarGrupo);

    document
        .getElementById("btnFiltrarGrupo")
        .addEventListener("click", () => {
            carregarGrupos();
        });

    document
        .getElementById("btnConfirmarExclusao")
        .addEventListener("click", excluirGrupo);
});

async function carregarGrupos() {
    const nome = document.getElementById("FiltroGrupoNome").value.trim();

    let url = `${GRUPO_API}?pageNumber=1&pageSize=10`;

    if (nome) {
        url += `&nome=${encodeURIComponent(nome)}`;
    }

    try {
        const response = await fetch(url);

        if (!response.ok) {
            mostrarToast("Erro ao carregar grupos", "danger");
            return;
        }

        const result = await response.json();
        const grupos = result.data.data;

        const tbody = document.getElementById("dadosTabelaGrupo");
        tbody.innerHTML = "";

        grupos.forEach(grupo => {
            tbody.innerHTML += `
                <tr>
                    <td>${grupo.nomeGrupo}</td>
                    <td>
                        <button class="btn btn-sm btn-warning me-1"
                            onclick="editarGrupo(${grupo.idGrupo})">
                            Editar
                        </button>
                        <button class="btn btn-sm btn-danger"
                            onclick="confirmarExcluirGrupo(${grupo.idGrupo})">
                            Excluir
                        </button>
                    </td>
                </tr>
            `;
        });

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado ao carregar grupos", "danger");
    }
}

async function editarGrupo(id) {
    try {
        const response = await fetch(`${GRUPO_API}/Listar/${id}`);

        if (!response.ok) {
            mostrarToast("Grupo não encontrado", "danger");
            return;
        }

        const result = await response.json();
        const grupo = result.data;

        document.getElementById("GrupoId").value = grupo.idGrupo;
        document.getElementById("GrupoNome").value = grupo.nomeGrupo;
        document.getElementById("tituloModalGrupo").innerText = "Editar Grupo";

        new bootstrap.Modal(
            document.getElementById("modalGrupo")
        ).show();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro ao buscar grupo", "danger");
    }
}

async function salvarGrupo() {
    const id = document.getElementById("GrupoId").value;
    const nomeGrupo = document.getElementById("GrupoNome").value.trim();

    if (!nomeGrupo) {
        mostrarToast("Informe o nome do grupo", "warning");
        return;
    }

    const payload = { NomeGrupo: nomeGrupo };

    const url = id
        ? `${GRUPO_API}/Editar/${id}`
        : `${GRUPO_API}/Criar`;

    const method = id ? "PUT" : "POST";

    try {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            mostrarToast("Erro ao salvar grupo", "danger");
            return;
        }

        mostrarToast(
            id ? "Grupo atualizado com sucesso!" : "Grupo criado com sucesso!",
            "success"
        );

        bootstrap.Modal.getInstance(
            document.getElementById("modalGrupo")
        ).hide();

        document.getElementById("formGrupo").reset();
        document.getElementById("GrupoId").value = "";

        carregarGrupos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}

function confirmarExcluirGrupo(id) {
    grupoExcluirId = id;

    new bootstrap.Modal(
        document.getElementById("modalConfirmacao")
    ).show();
}

async function excluirGrupo() {
    if (!grupoExcluirId) return;

    try {
        const response = await fetch(
            `${GRUPO_API}/Deletar/${grupoExcluirId}`,
            { method: "DELETE" }
        );

        if (!response.ok) {
            mostrarToast("Erro ao excluir grupo", "danger");
            return;
        }

        bootstrap.Modal.getInstance(
            document.getElementById("modalConfirmacao")
        ).hide();

        mostrarToast("Grupo excluído com sucesso!", "success");
        grupoExcluirId = null;

        carregarGrupos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}
