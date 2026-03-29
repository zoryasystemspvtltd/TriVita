import pathlib
root = pathlib.Path(r"i:/Projects/TriVita/HealthcarePlatform/HMSService/HMSService.API/Controllers")
old = "[SwaggerResponse(StatusCodes.Status200OK, typeof(BaseResponse"
new = '[SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse'
for p in root.rglob("*.cs"):
    t = p.read_text(encoding="utf-8")
    if old not in t:
        continue
    p.write_text(t.replace(old, new), encoding="utf-8")
    print("fixed", p)
