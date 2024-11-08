from llama_cpp import Llama

MODEL_PATH = "../Phi-3-mini-4k-instruct-q4.gguf"

llm = Llama(
	model_path=MODEL_PATH,
	# n_gpu_layers=-1, # Uncomment to use GPU acceleration
	# seed=1337, # Uncomment to set a specific seed
	# n_ctx=2048, # Uncomment to increase the context window
)
output = llm(
	"Q: Name the planets in the solar system? A: ", # Prompt
	max_tokens=32, # Generate up to 32 tokens, set to None to generate up to the end of the context window
	stop=["Q:", "\n"], # Stop generating just before the model would generate a new question
	echo=True # Echo the prompt back in the output
) # Generate a completion, can also call create_completion
print(output)

llm = Llama(
	model_path=MODEL_PATH,
	chat_format="chatml" # Other format: chatml, gemma, ecc. See llama_chat_format.py and search for "@register_chat_format"
)
response = llm.create_chat_completion(
	stream=True,
	messages = [
		{
			"role": "system",
			"content": "You are an assistant who talks about astronomy."
		},
		{
			"role": "user",
			"content": "Name the planets in the solar system."
		}
	]
)
output = ""
for message in response:
	output += message["content"] + "\n"
print(output)
