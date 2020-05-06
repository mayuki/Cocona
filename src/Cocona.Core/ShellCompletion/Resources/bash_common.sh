__cocona_APPNAMEPLACEHOLDER_completion_define_command() {
    local command_name=$1

    commands+=($command_name)
}

__cocona_APPNAMEPLACEHOLDER_completion_define_argument() {
    local argument_name=$1
    local argument_type=$2
    local argument_candidates_provider=$3

    arguments+=($argument_name)
    argument_types+=($argument_type)
    argument_candidates_providers+=(${argument_candidates_provider:-''})
}

__cocona_APPNAMEPLACEHOLDER_completion_define_option() {
    local option_name=$1
    local option_type=$2
    local option_candidates_provider=$3

    options+=($option_name)
    option_types+=($option_type)
    option_candidates_providers+=(${option_candidates_provider:-''})
}

__cocona_APPNAMEPLACEHOLDER_completion_contains() {
    local value="$1"
    for i in "${@:2}"
    do
        if [[ "$i" == "$value" ]]; then
            return 0
        fi
    done

    return 1
}

__cocona_APPNAMEPLACEHOLDER_completion_debug_log() {
    #echo $*
    return 0
}

__cocona_APPNAMEPLACEHOLDER_completion_get_argument_index() {
    local params=(${words[@]:((${#cur_command_stack[@]}))})
    local index=0
    local options_completed=0
    local prev_is_option=0

    for param in "${params[@]}"
    do
        __cocona_APPNAMEPLACEHOLDER_completion_debug_log "$param" index=$index options_completed=$options_completed prev_is_option=$prev_is_option
        if [[ $options_completed -eq 1 ]]; then
            ((index++))
            prev_is_option=0
        elif [[ "$param" = '--' ]]; then
            options_completed=1
            prev_is_option=0
        elif [[ "${param:0:2}" = '--' ]]; then
            if [[ "$param" != *=* ]]; then
                prev_is_option=1
            fi
        elif [[ $prev_is_option -eq 0 ]]; then
            ((index++))
            prev_is_option=0
        else
            prev_is_option=0
        fi
    done

    return $index
}

__cocona_APPNAMEPLACEHOLDER_completion_index_of() {
    local value="$1"
    local index=0
    for i in "${@:2}"
    do
        if [[ "$i" == "$value" ]]; then
            return $index
        fi
        ((index++))
    done

    return 255
}

__cocona_APPNAMEPLACEHOLDER_completion_handle() {
    local command_depth=${#cur_command_stack[@]}

    # subcommands
    if [[ ${#words} -gt 1 && ${#commands} -gt 0 ]]; then
        __cocona_APPNAMEPLACEHOLDER_completion_debug_log "__cocona_APPNAMEPLACEHOLDER_completion_contains ${words[$command_depth]}" "${commands[@]}"
        if __cocona_APPNAMEPLACEHOLDER_completion_contains "${words[$command_depth]}" "${commands[@]}"; then
            cur_command_stack+=("${words[$command_depth]}")
            local next
            next="$(IFS=_; echo "${cur_command_stack[*]}")"
            
            __cocona_APPNAMEPLACEHOLDER_completion_init_variables
            __cocona_APPNAMEPLACEHOLDER_completion_debug_log "__cocona_APPNAMEPLACEHOLDER_commands_$next"
            eval "__cocona_APPNAMEPLACEHOLDER_commands_$next"
            return 0
        fi
        if [[ $cword -eq $command_depth ]]; then
            __cocona_APPNAMEPLACEHOLDER_completion_set_candidates "${commands[@]}"
            return 0
        fi
    fi

    # --option value
    __cocona_APPNAMEPLACEHOLDER_completion_index_of "$prev" "${options[@]}"
    local index_of_option=$?
    if [[ $index_of_option -ne 255 ]]; then
        case ${option_types[$index_of_option]} in
            default)
                __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default
                return 0
                ;;
            file)
                __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_file
                return 0
                ;;
            dir)
                __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_dir
                return 0
                ;;
            enum*)
                local keywords_str=${option_types[$index_of_option]#enum:}
                local keywords=(${keywords_str//:/ })
                __cocona_APPNAMEPLACEHOLDER_completion_set_candidates "${keywords[@]}"
                return 0
                ;;
            bool)
                # continue to next argument or option
                ;;
            * | default)
                __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default
                return 0
                ;;
        esac
    fi

    # --option
    if [[ "${cur:0:1}" = '-' ]]; then
        __cocona_APPNAMEPLACEHOLDER_completion_set_candidates "${options[@]}"
        return 0
    fi

    # arguments...
    __cocona_APPNAMEPLACEHOLDER_completion_get_argument_index
    local index_of_arg=$?
    __cocona_APPNAMEPLACEHOLDER_completion_debug_log "argument" index_of_arg=$index_of_arg "type=${argument_types[$index_of_arg]}"
    case ${argument_types[$index_of_arg]} in
        default)
            __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default
            return 0
            ;;
        file)
            __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_file
            return 0
            ;;
        dir)
            __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_dir
            return 0
            ;;
        * | default)
            __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default
            return 0
            ;;
    esac

    __cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default
    return 0
}

__cocona_APPNAMEPLACEHOLDER_completion_set_candidates() {
    __cocona_APPNAMEPLACEHOLDER_completion_debug_log "__cocona_APPNAMEPLACEHOLDER_completion_set_candidates" "$@"

    local candidates
    candidates="$(IFS=' '; echo "${*:1}")"
    COMPREPLY=($(compgen -W "${candidates}" -- "${cur}"))
}
__cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_default() {
    COMPREPLY=($(compgen -o default -f -- "${cur}"))
}
__cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_file() {
    COMPREPLY=($(compgen -f -- "${cur}"))
}
__cocona_APPNAMEPLACEHOLDER_completion_set_candidates_for_dir() {
    COMPREPLY=($(compgen -d -- "${cur}"))
}

__cocona_APPNAMEPLACEHOLDER_completion_init_variables() {
    commands=()
    options=()
    option_types=()
    option_candidates_providers=()
}

__cocona_APPNAMEPLACEHOLDER_completion_entrypoint() {
    local cur prev words cword
    _init_completion -s || return

    #echo cur=$cur
    #echo prev=$prev
    #echo words=${words[@]}
    #echo cword=$cword

    local cur_command_stack=(root)
    local commands=()
    local options=()
    local option_types=()
    local option_candidates_providers=()
    local arguments=()
    local argument_types=()
    local argument_candidates_providers=()

    __cocona_APPNAMEPLACEHOLDER_commands_root
}

__cocona_APPNAMEPLACEHOLDER_completion_entrypoint

complete -F __cocona_APPNAMEPLACEHOLDER_completion_entrypoint APPCOMMANDNAMEPLACEHOLDER