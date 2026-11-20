import { ContentSection } from "../ContentSection/ContentSection.jsx";

export function DocPage({ page }) {
  return (
    <>
      <div className="page-intro">
        <div className="eyebrow"><span />{page.eyebrow}</div>
        <h1>{page.title}</h1>
        <p>{page.description}</p>
      </div>
      <div className="page-content">
        {page.sections.map((section, index) => (
          <ContentSection index={index} key={section.title} section={section} />
        ))}
        <div className="page-feedback">
          <span>Esta documentação prioriza comportamentos confirmados no código.</span>
          <a
            href="https://github.com/rafael-serafin1/ptml"
            rel="noreferrer"
            target="_blank"
          >
            Ver repositório <span aria-hidden="true">↗</span>
          </a>
        </div>
      </div>
    </>
  );
}