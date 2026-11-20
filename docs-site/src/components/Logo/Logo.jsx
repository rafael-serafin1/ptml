import { Link } from "react-router-dom";

export function Logo() {
  return (
    <Link aria-label="PTML — página inicial" className="brand" to="/">
      <span className="brand-mark">&lt;/&gt;</span>
      <span>PTML<span className="brand-period">.</span></span>
    </Link>
  );
}