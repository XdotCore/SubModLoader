ifdef rax
	.data
		extern trueFuncPtr : qword
	.code
		callTrueFuncPtrASM proc
			jmp qword ptr [trueFuncPtr]
		callTrueFuncPtrASM endp
else
	.model flat, C
	.data
		extern trueFuncPtr : dword
	.code
		callTrueFuncPtrASM proc
			jmp dword ptr [trueFuncPtr]
		callTrueFuncPtrASM endp
endif
end