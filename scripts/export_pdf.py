from pathlib import Path
from textwrap import wrap

from fpdf import FPDF


def add_markdown_to_pdf(pdf: FPDF, markdown_text: str) -> None:
    """
    Conversão simples de Markdown para PDF.
    Suporta títulos (#, ##, ###) e listas com "-".
    """
    lines = markdown_text.splitlines()

    for raw_line in lines:
        line = raw_line.rstrip()

        if not line:
            pdf.ln(5)
            continue

        if line.startswith("### "):
            content = line[4:].strip()
            pdf.set_font("Helvetica", "B", 12)
            pdf.multi_cell(0, 8, content)
            pdf.ln(2)
        elif line.startswith("## "):
            content = line[3:].strip()
            pdf.set_font("Helvetica", "B", 14)
            pdf.multi_cell(0, 9, content)
            pdf.ln(3)
        elif line.startswith("# "):
            content = line[2:].strip()
            pdf.set_font("Helvetica", "B", 16)
            pdf.multi_cell(0, 10, content)
            pdf.ln(4)
        elif line.startswith("- "):
            content = line[2:].strip()
            pdf.set_font("Helvetica", "", 11)
            pdf.multi_cell(0, 7, f"• {content}")
        elif line.startswith("```") and line.endswith("```"):
            # Ignorar blocos de código de uma linha
            continue
        elif line.startswith("```"):
            pdf.set_font("Courier", "", 10)
        elif line.endswith("```"):
            pdf.set_font("Helvetica", "", 11)
        elif line.startswith("    ") or line.startswith("\t"):
            pdf.set_font("Courier", "", 10)
            pdf.multi_cell(0, 6, line.strip())
        else:
            pdf.set_font("Helvetica", "", 11)
            wrapped = wrap(line, width=110)
            if not wrapped:
                wrapped = [""]
            for segment in wrapped:
                pdf.multi_cell(0, 6, segment)


def export_markdown_to_pdf(markdown_path: Path, output_pdf: Path) -> None:
    text = markdown_path.read_text(encoding="utf-8")

    pdf = FPDF(orientation="P", unit="mm", format="A4")
    pdf.set_auto_page_break(auto=True, margin=15)
    pdf.add_page()
    pdf.set_margins(left=15, top=15, right=15)

    add_markdown_to_pdf(pdf, text)

    output_pdf.parent.mkdir(parents=True, exist_ok=True)
    pdf.output(str(output_pdf))


def main() -> None:
    root = Path(__file__).resolve().parents[1]
    markdown_path = root / "docs" / "Guia_PagueVeloz.md"
    output_pdf = root / "docs" / "Guia_PagueVeloz.pdf"

    if not markdown_path.exists():
        raise FileNotFoundError(f"Arquivo Markdown não encontrado: {markdown_path}")

    export_markdown_to_pdf(markdown_path, output_pdf)
    print(f"PDF gerado em: {output_pdf}")


if __name__ == "__main__":
    main()


