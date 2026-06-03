package require Tk

text .output -width 100 -height 30
pack .output -fill both -expand 1

entry .input
pack .input -fill x

bind .input <Return> {
    set cmd [.input get]
    .input delete 0 end

    .output insert end "> $cmd\n"

    if {[catch {
        exec {*}[split $cmd]
    } result]} {
        .output insert end "Erro: $result\n"
    } else {
        .output insert end "$result\n"
    }

    .output see end
}