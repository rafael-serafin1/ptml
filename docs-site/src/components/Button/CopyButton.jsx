import {useState} from "react";

export function CopyButton({ code }) {
  const [status, setStatus] = useState("");

  async function copyCode() {
    try {
      await navigator.clipboard.writeText(code);
      setStatus("Copiado");
      window.setTimeout(() => setStatus(""), 1800);
    } catch {
      setStatus("Não foi possível copiar");
    }
  }

  return (
    <button
      aria-label="Copiar exemplo"
      className="copy-button focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-green-700"
      onClick={copyCode}
      type="button"
    >
      {status || "Copiar"}
    </button>
  );
}