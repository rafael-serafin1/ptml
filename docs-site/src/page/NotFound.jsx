import { Link } from "react-router-dom";

export function NotFound() {
  return (
    <div className="not-found">
      <span>404</span>
      <h1>Página não encontrada</h1>
      <p>Esse endereço não corresponde a uma página desta documentação.</p>
      <Link className="button-primary focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-green-700" to="/">Voltar ao início <span aria-hidden="true">→</span></Link>
    </div>
  );
}